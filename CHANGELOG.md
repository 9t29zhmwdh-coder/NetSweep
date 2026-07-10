# Changelog

All notable changes to NetSweep will be documented here.
Format based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

## [0.2.0] - 2026-07-10

### Added

- Real README screenshot (`docs/screenshot.png`), generated headlessly via a new `--screenshot <path>` startup flag that renders the main window off-screen (`RenderTargetBitmap`) and exits; see `.github/workflows/screenshot.yml` for the manually-triggered CI job that produces it
- "New here?" beginner guide callout to README.de.md (was missing; README.md already had it)

## [0.1.1] - 2026-07-08

### Fixed

- Corrected `ARCHITECTURE.md`'s file tree and CI snippet to match the actual `NetSweep/` layout and the real `.github/workflows/build.yml`, and removed an overclaimed "undo capability" for quarantine (it relocates files for manual recovery, there is no automated undo action)
- Corrected `CHANGELOG.md`'s (this file) description of the initial release to match the actually shipped features
- Corrected `README.md`'s Project Structure section, which listed `RelayCommand` under `ViewModels/` (it's in `Helpers/`) and a model named `Connection` (the actual class is `StorageConnection`)
- Corrected `project.yaml`'s `license: TBD` (a `LICENSE` file with MIT already exists) and the same `RelayCommand` misplacement
- Fixed `CONTRIBUTING.md` instructing contributors to run `dotnet test`; there is no test project in the solution
- Fixed em-dashes across documentation, `project.yaml`, and a source comment
- Removed stale `SKELETON.md`/`TEMPLATE_NOTES.md` scaffolding bookkeeping

## [0.1.0] - 2026-06-15

### Added

- Connection management for NAS, UNC, DFS namespace, and mapped SharePoint/OneDrive for Business paths
- Storage visualization: aggregated usage per folder with size breakdown
- File filters by age, size, extension, and filename pattern
- Duplicate detection via SHA-256 hash comparison (size-filtered first)
- Empty folder listing and batch cleanup
- File actions: permanent delete (double confirmation), move-to-quarantine, copy/backup, CSV export
- Credential encryption via Windows DPAPI (`CurrentUser` scope)
- WPF / .NET 8 desktop UI
