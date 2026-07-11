# Roadmap

## v0.1.0, Initial Release (2026-06-15)

- Connection management for NAS, UNC, DFS namespace, and mapped SharePoint/OneDrive for Business paths
- Storage visualization with aggregated per-folder usage
- File filters by age, size, extension, and filename pattern
- Duplicate file detection via SHA-256 hash comparison
- Empty folder listing and batch cleanup
- Quarantine (move-before-delete) and permanent delete with double confirmation
- CSV export (semicolon-separated for Excel CH/DE locales)
- Credential encryption via Windows DPAPI

## v0.2.0, Planned

- [ ] Scheduled / automatic scans with email notification
- [ ] HTML report export (CSV export already shipped in v0.1.0)
- [ ] Automated test project (the solution currently has no test project at all)

## v0.3.0, Planned

- [ ] Network drive health check
- [ ] Multi-drive comparison view
- [ ] Incremental backup with versioning

## v1.0.0, Stable

- [ ] Full test coverage
- [ ] MSIX installer / Intune deployment package
- [ ] Localization (DE/EN)
- [ ] SharePoint Online drive mapping via Microsoft Graph API (current support is via mapped UNC paths only, not the Graph API)
- [ ] OneDrive for Business quota reporting
- [ ] Microsoft Purview data lifecycle policy compliance check

## Dual-Licensing Readiness

Assessed 2026-07-11 as a Dual-Licensing candidate (Community MIT + Commercial/Enterprise tier): NetSweep is already explicitly positioned for enterprise Microsoft environments and aligned with Microsoft Purview data lifecycle management, and its own roadmap lists several classic enterprise differentiators. Not ready yet; blocked on:

- [ ] No centralized or multi-share reporting yet: each run audits the shares one admin connects to, there is no org-wide storage governance view
- [ ] SharePoint Online via Microsoft Graph API, OneDrive for Business quota reporting, and the Purview compliance check are still only roadmap entries, not implemented
- [ ] No scheduled/automated scanning with centralized alerting yet (v0.2.0 item above)
- [ ] No MSIX/Intune deployment package yet, which would be the natural enterprise distribution path

Once the Graph API integration and Purview compliance check (v1.0.0) land, revisit: candidate Enterprise-only features would be org-wide storage governance reporting across shares, Purview compliance dashboarding, and Intune-deployed scheduled scans with centralized alerting, with the core connection management, storage visualization and duplicate/cleanup engine staying Community/MIT.
