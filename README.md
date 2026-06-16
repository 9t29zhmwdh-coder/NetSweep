<div align="center">
  <img src="RayStudio.png" alt="RayStudio Logo" width="120"/>

  <h1>NetSweep – Network Storage Cleanup</h1>
</div>

> 🇩🇪 [Deutsche Version](README.de.md)

A Windows desktop application (WPF, .NET 8) for auditing and cleaning up network drives — NAS, UNC paths, mapped SharePoint libraries, and DFS namespaces. Manage connections, visualize storage usage, detect duplicates, and remove stale files with full audit-trail support.

[![CI](https://github.com/9t29zhmwdh-coder/NetSweep/actions/workflows/build.yml/badge.svg)](https://github.com/9t29zhmwdh-coder/NetSweep/actions)
![.NET](https://img.shields.io/badge/.NET-8-orange?logo=dotnet)
![WPF](https://img.shields.io/badge/WPF-.NET%208-blue?logo=windows)
![Platform](https://img.shields.io/badge/Platform-Windows-lightgrey?logo=windows)
![License](https://img.shields.io/badge/License-MIT-green)

---

## Features

| Feature | Description |
|---------|-------------|
| **Connection Management** | Add, edit and connect to multiple NAS / UNC / DFS / SharePoint-mapped paths |
| **Storage Visualization** | Aggregated storage usage per folder with percentage share and size breakdown |
| **File Filters** | Filter by age (days), size, extension or filename pattern |
| **Duplicate Detection** | SHA-256 hash comparison — see exactly how much space is recoverable |
| **Empty Folder Cleanup** | List and batch-remove empty directory trees |
| **File Actions** | Permanently delete (double confirmation), quarantine to staging path, copy/backup, CSV export |

---

## Enterprise Use Cases

- **SharePoint / OneDrive for Business** — scan mapped SharePoint document libraries via UNC or drive letter; identify oversized, outdated, or duplicate files before migration
- **DFS Namespace Support** — connect to `\\domain\dfs\...` paths as standard UNC connections
- **Pre-Migration Auditing** — export CSV inventories for file-share to SharePoint or OneDrive migrations
- **Storage Governance** — schedule reviews of network shares and generate exportable reports for IT operations

---

## Microsoft Ecosystem Compatibility

| Component | Support |
|-----------|---------|
| Windows 10 / 11 | Native WPF app |
| SharePoint mapped drives | Full — via mapped UNC path |
| OneDrive for Business | Full — via sync folder or mapped library |
| DFS Namespaces | Full — standard UNC resolution |
| Windows DPAPI | Credential encryption at rest |
| Entra ID / AD joined devices | Works on domain-joined and AAD-joined machines |

---

## Security

- Credentials are encrypted with **Windows DPAPI** (CurrentUser scope) — never stored in plain text
- Connection profiles stored at `%AppData%\NetSweep\connections.json` — excluded from version control
- **Permanent delete has no undo** — requires double confirmation; quarantine option available
- Designed for **least-privilege accounts**: read + write access only to the targeted share
- No outbound network calls — fully offline operation

---

## Requirements

- Windows 10 / 11
- [.NET 8 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0) (or publish self-contained)
- Visual Studio 2022 (17.8+) with **.NET Desktop Development** workload *(for building)*

---

## Getting Started

```bash
# Open solution
NetSweep.sln   # → Visual Studio → F5

# CLI build
dotnet build
dotnet run --project NetSweep

# Self-contained single-file publish
dotnet publish NetSweep/NetSweep.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

---

## Project Structure

```
NetSweep.sln
└─ NetSweep/
   ├─ App.xaml(.cs)       Entry point — Welcome → Main window
   ├─ Models/             Data models (Connection, FileEntry, FolderNode, ScanResult)
   ├─ Services/           Business logic (Scan, Duplicates, FileOps, Encryption, CSV)
   ├─ ViewModels/         MVVM (MainViewModel, AnalysisViewModel, RelayCommand)
   ├─ Views/              XAML windows (Welcome, Main, ConnectionEdit, Analysis)
   └─ Helpers/            Utilities (ByteSize formatting, path normalization)
```

---

## Roadmap

- [ ] Scheduled / automatic scans with email notification
- [ ] Incremental backup with versioning
- [ ] Microsoft Graph API integration for SharePoint inventory
- [ ] Audit log for all delete / move actions (CSV + Event Log)
- [ ] File type statistics with chart visualization
- [ ] Parallel scan of multiple paths
- [ ] Intune / SCCM deployment package (MSIX)

---

**Author:** [Rafael Yilmaz](https://github.com/9t29zhmwdh-coder) &nbsp;·&nbsp; **Status:** Active &nbsp;·&nbsp; **Last Updated:** June 2026
