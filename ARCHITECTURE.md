# Architecture

## Overview

NetSweep is a Windows desktop application for scanning and cleaning network drives,
built with WPF and .NET 8 using MVVM architecture.

```
NetSweep/
├── App.xaml                  # Application entry
├── MainWindow.xaml           # Shell / navigation
├── ViewModels/
│   ├── ScanViewModel.cs
│   └── QuarantineViewModel.cs
├── Views/
│   ├── ScanView.xaml         # TreeSize-style tree view
│   └── QuarantineView.xaml
├── Services/
│   ├── FileSystemScanner.cs
│   ├── QuarantineService.cs
│   └── FileCopyService.cs
├── Models/
│   └── FileNode.cs
└── Themes/
    └── Dark.xaml
```

## Design Decisions

- **WPF TreeView:** Native Windows control for hierarchical file display.
- **Quarantine model:** Move-before-delete for safe cleanup with undo capability.
- **MVVM:** Fully testable ViewModels separate from UI.

## CI

```yaml
name: CI
on: [push, pull_request]
jobs:
  build:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with: {dotnet-version: '8.0.x'}
      - run: dotnet restore
      - run: dotnet build --no-restore
      - run: dotnet test --no-build
```
