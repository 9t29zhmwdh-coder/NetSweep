# Changelog

All notable changes to NetSweep will be documented here.
Format based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

## [1.0.4] - 2026-07-29

### Changed

Dependency and workflow updates merged since 1.0.3:

- chore(ci): bump the actions group across 1 directory with 3 updates
- chore(deps): Bump coverlet.collector and 3 others

---

## [1.0.3] - 2026-07-28

### Changed

- CodeQL moved from GitHub's default setup to an advanced setup with a committed `.github/workflows/codeql.yml`. The default setup skips pull requests that touch no code of a given language, so a dependency pull request changing only a lock file reported `skipping` on the required checks forever and could never be merged. The workflow runs on every pull request regardless of what changed and uses the `security-extended` query suite, which the default setup does not allow choosing. Required checks are unchanged.

## [1.0.2] - 2026-07-28

### Added

- `.github/dependabot.yml`, with grouped weekly updates. The file was missing, and without it there are no version updates at all: repository security alerts only fire for disclosed vulnerabilities. Follows `engineering-standards` v0.10.0.

### Fixed

- 8 action references used a mutable tag or branch instead of a commit SHA, `dtolnay/rust-toolchain@stable` among them where applicable. A branch HEAD can be moved to point at different code at any time. All are now pinned, at the version that was actually running rather than upgraded, so any major bump arrives as its own reviewable Dependabot PR.
- Only `NetSweep/NetSweep.csproj` carries a version; the test project inherits none, which is correct for a project that is never packaged.

## [1.0.1] - 2026-07-20

### Changed

- OpenSSF Scorecard workflow and badge.
- `copilot-instructions.md` for consistent AI-assisted contributions.
- Restored real German umlauts in UI localization strings.
- Initial xUnit suite for the scan/classification/cleanup logic (25 tests) with coverage reporting in CI.
- Split the README's security/CI badges onto their own line, separate from the platform/tech/AI badges (they were rendering as a single merged line).

## [1.0.0] - 2026-07-17

First stable release: a real, packaged, installable distribution exists
for end users. Real Windows installer (Inno Setup), the only platform this WPF/.NET app targets.

## [0.3.9] - 2026-07-17

### Changed
- CI: added an explicit `permissions: contents: read` block to the workflow(s) that were missing one (CodeQL `actions/missing-workflow-permissions`), narrowing the default GITHUB_TOKEN scope.

## [0.3.8] - 2026-07-13

### Added

- README.md/README.de.md: "How it runs" callout, "In practice" paragraph, and "Uninstall/Cleanup" section, which this repo was missing entirely in both languages.

## [0.3.7] - 2026-07-12

### Fixed

- Removed em-dashes and en-dashes from GETTING_STARTED.md. Swiss German orthography rule.

## [0.3.6] - 2026-07-12

### Added

- TERMS_OF_SALE.md: terms covering the purchase of a pre-built, packaged distribution through a marketplace (as-is, no warranty, liability strictly capped at the amount paid). Does not modify the existing MIT LICENSE, which continues to cover the source code at no cost.

## [0.3.5] - 2026-07-12

### Added

- Dual-Licensing skeleton: LICENSE.COMMERCIAL, COMMERCIAL.md, and ENTERPRISE_FEATURES.md, documenting the licensing model for a future Enterprise Edition ahead of any actual feature split. The existing MIT LICENSE and all currently released code are unchanged; nothing in this repository is restricted by this addition.

## [0.3.4] - 2026-07-11

### Fixed

- Removed a leftover employer reference from the project's initial commit: `Company`/`Publisher` metadata in `NetSweep.csproj` and `installer.iss` now correctly identify RayStudio/Rafael Yilmaz as the author, not a third party.

## [0.3.3] - 2026-07-11

### Added

- Documented Dual-Licensing readiness assessment in ROADMAP.md.

## [0.3.2] - 2026-07-10

### Fixed

- Removed em-dash from the download callout in README.md/README.de.md, replaced with a colon

## [0.3.1] - 2026-07-10

### Added

- Real EN/DE screenshots regenerated with the new language toggle (`docs/screenshot.png`, `docs/screenshot.de.png`)
- Installer download link in README.md/README.de.md pointing at the latest release's `NetSweep-Setup.exe`

## [0.3.0] - 2026-07-10

### Added

- EN/DE language toggle: English is now the default UI language, German is a toggle option (button in the top-right corner). All UI text, status messages and dialogs across MainWindow, WelcomeWindow, ConnectionEditDialog and AnalysisWindow are now localized (`Helpers/Loc.cs`)
- `.github/workflows/installer.yml`: automatically builds an Inno Setup installer (`NetSweep-Setup.exe`) and attaches it to the GitHub Release whenever a release is published. Not code-signed: Windows SmartScreen shows an "Unknown Publisher" warning on first run (accepted trade-off for this project, no certificate)
- `--screenshot` startup flag now accepts an optional language argument; the screenshot workflow produces both `docs/screenshot.png` (EN) and `docs/screenshot.de.png` (DE)

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
