<div align="center">
  <img src="RayStudio.png" alt="RayStudio Logo" width="120"/>

  <h1>NetSweep – Network Storage Cleanup</h1>
</div>

> 🇩🇪 [Deutsche Version](README.de.md)

# NetSweep – Network Storage Cleanup

A Windows desktop app (WPF, .NET 8) for scanning and cleaning up network drives (NAS / UNC paths). Manage connections, visualize storage usage per folder (TreeSize-style), find old/large/duplicate files, remove empty folders, and copy, quarantine, or permanently delete files.

## Features

- **Connection Management** — Add, edit and connect to multiple NAS/UNC paths
- **TreeSize View** — Aggregated storage usage per folder with percentage share
- **File Filters** — Filter by age (days), size, extension or filename
- **Duplicate Detection** — Find identical files via SHA-256 hash, see reclaimable space
- **Empty Folders** — List and remove empty directories
- **Actions** — Permanently delete (double confirmation), quarantine, copy/backup, CSV export

## Requirements

- Windows 10 / 11
- [.NET 8 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0) (or publish self-contained)
- Visual Studio 2022 (17.8+) with **.NET Desktop Development** workload *(for building)*

## Getting Started

```bash
# Open solution
NetSweep.sln   # → Visual Studio → F5

# Or via CLI
dotnet build
dotnet run --project NetSweep
```

**Publish standalone .exe:**
```bash
dotnet publish NetSweep/NetSweep.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

## Project Structure

```
NetSweep.sln
└─ NetSweep/
   ├─ App.xaml(.cs)       App entry point, Welcome → Main window
   ├─ Models/             Data models (Connection, FileEntry, FolderNode, ScanResult)
   ├─ Services/           Business logic (Scan, Duplicates, FileOps, Encryption, CSV)
   ├─ ViewModels/         MVVM (MainViewModel, AnalysisViewModel, RelayCommand)
   ├─ Views/              XAML windows (Welcome, Main, ConnectionEdit, Analysis)
   └─ Helpers/            Utilities (ByteSize formatting)
```

## Security

- Credentials are encrypted with **Windows DPAPI** (CurrentUser scope) — never stored in plain text
- Stored at `%AppData%\NetSweep\connections.json` — excluded from version control
- **Delete is permanent** (no recycle bin) — confirmed twice before execution
- Recommendation: use a restricted NAS account with write access only to the relevant folders

## Roadmap

- [ ] Scheduled / automatic scans
- [ ] Incremental backup with versioning
- [ ] Move to Windows Recycle Bin as option
- [ ] File type statistics (chart)
- [ ] Scan multiple paths simultaneously
- [ ] Audit log for all delete/move actions

---

<div align="right">
  <sub>by</sub><br/>
  <img src="RayStudio.png" alt="RayStudio" width="70"/>
</div>

**Author:** [Rafael Yilmaz](https://github.com/9t29zhmwdh-coder) &nbsp;·&nbsp; **Status:** Early Release &nbsp;·&nbsp; **Last Updated:** June 2026
