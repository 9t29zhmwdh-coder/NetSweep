# NetSweep — Professional Repo Skeleton

**Generated:** 2026-06-16 | **Earliest commit:** 2026-06-15 | **Release:** v0.1.0

## Files Added

- SKELETON.md ✅
- ARCHITECTURE.md ✅
- PRIVACY.md ✅
- ROADMAP.md ✅
- CONTRIBUTING.md ✅
- CODE_OF_CONDUCT.md ✅
- SECURITY.md ✅
- CHANGELOG.md ✅
- .github/ISSUE_TEMPLATE/bug_report.md ✅
- .github/ISSUE_TEMPLATE/feature_request.md ✅
- .github/PULL_REQUEST_TEMPLATE.md ✅
- .github/workflows/ci.yml ⚠️ requires `workflows` OAuth scope

## CI Workflow

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

## Canonical File Tree

```
NetSweep/
├── NetSweep/
│   ├── App.xaml
│   ├── MainWindow.xaml
│   ├── ViewModels/
│   ├── Views/
│   ├── Services/
│   ├── Models/
│   └── Themes/
├── NetSweep.Tests/
├── ARCHITECTURE.md
├── CHANGELOG.md
├── CODE_OF_CONDUCT.md
├── CONTRIBUTING.md
├── LICENSE
├── PRIVACY.md
├── README.md
├── ROADMAP.md
├── SECURITY.md
└── SKELETON.md
```

---
*NetSweep — RayStudio · Rafael Yilmaz · MIT License · 2026*
