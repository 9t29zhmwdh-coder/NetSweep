# Architecture

## Overview

NetSweep is a Windows desktop application for auditing and cleaning up network drives, built with WPF and .NET 8 using MVVM architecture.

```
NetSweep.sln
└─ NetSweep/
   ├── App.xaml(.cs)                 # Entry point: Welcome window, then Main window
   ├── ViewModels/
   │   ├── ViewModelBase.cs
   │   ├── MainViewModel.cs
   │   ├── AnalysisViewModel.cs
   │   └── Converters.cs
   ├── Views/
   │   ├── WelcomeWindow.xaml
   │   ├── MainWindow.xaml
   │   ├── ConnectionEditDialog.xaml
   │   └── AnalysisWindow.xaml
   ├── Services/
   │   ├── ScanService.cs             # Directory tree walk, aggregated storage usage
   │   ├── DuplicateFinder.cs         # SHA-256 based duplicate detection
   │   ├── FileOperationService.cs    # Permanent delete, move-to-quarantine
   │   ├── ConnectionStore.cs         # Persists connection profiles (%AppData%\NetSweep)
   │   ├── CredentialService.cs       # DPAPI encryption for stored credentials
   │   ├── NetworkConnectionService.cs
   │   └── ReportService.cs           # CSV export
   ├── Models/
   │   ├── StorageConnection.cs
   │   ├── FileEntry.cs
   │   ├── FolderNode.cs
   │   ├── ScanResult.cs
   │   └── DuplicateGroup.cs
   └── Helpers/
       ├── ByteSize.cs
       ├── PathBuilder.cs
       └── RelayCommand.cs
```

## Design Decisions

- **MVVM:** ViewModels contain no WPF/UI types, so scan, duplicate-detection, and file-operation logic can be exercised independently of the views.
- **Quarantine over permanent delete by default:** `FileOperationService.MoveToQuarantineAsync` relocates files into a quarantine folder, preserving their relative sub-path, so a mistaken cleanup is recoverable by moving files back manually. Permanent delete is a separate, explicit action gated behind a double-confirmation dialog in the UI.
- **DPAPI for credentials:** connection credentials are encrypted with `ProtectedData.Protect`/`Unprotect` (`DataProtectionScope.CurrentUser`), never stored in plain text, and never leave the machine (no outbound network calls anywhere in the codebase).
- **SHA-256 duplicate detection:** files are compared by size first (a cheap, decisive filter), then hashed only when sizes match, avoiding hashing every file in a large tree.

## CI

```yaml
name: Build NetSweep

on:
  push:
    branches: [main]
  pull_request:
    branches: [main]
  workflow_dispatch:

jobs:
  build:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v6
      - uses: actions/setup-dotnet@v5
        with:
          dotnet-version: '8.0.x'
      - run: dotnet restore NetSweep.sln
      - run: dotnet build NetSweep.sln -c Release --no-restore
      - name: Publish (self-contained, single file)
        run: >-
          dotnet publish NetSweep/NetSweep.csproj
          -c Release -r win-x64 --self-contained true
          -p:PublishSingleFile=true -o publish
      - uses: actions/upload-artifact@v7
        with:
          name: NetSweep-win-x64
          path: publish/NetSweep.exe
```

There is currently no automated test project in the solution (`NetSweep.sln` has a single `NetSweep.csproj`); CI builds and publishes but does not run `dotnet test`. Tracked in [ROADMAP.md](ROADMAP.md).
