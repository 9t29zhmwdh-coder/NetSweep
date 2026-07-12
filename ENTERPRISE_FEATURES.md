# Enterprise Features

This document lists features planned for the Enterprise Edition of this
project, licensed separately under
[LICENSE.COMMERCIAL](LICENSE.COMMERCIAL). See [COMMERCIAL.md](COMMERCIAL.md)
for the licensing model.

## Status

No Enterprise features have shipped yet. This list is a forward-looking plan,
not a changelog of existing functionality: everything currently in this
repository is part of the Community Edition and remains MIT-licensed. See the
repository's own [ROADMAP.md](ROADMAP.md), "Dual-Licensing Readiness"
section, for the prerequisites that need to land first.

## Planned

- Org-wide storage governance reporting: consolidated visibility across all
  shares an organization manages, instead of one admin's current connection.
- SharePoint Online (Microsoft Graph API) and OneDrive for Business quota
  reporting, alongside the existing on-premises share support.
- Microsoft Purview compliance dashboarding: surfacing data lifecycle and
  compliance findings alongside cleanup recommendations.
- Intune-deployed scheduled scans with centralized alerting, instead of
  manually triggered, single-run audits.

## Not planned

The core connection management, storage visualization, and duplicate/cleanup
engine stay in the Community Edition permanently. Dual-licensing governs only
new, enterprise-shaped capabilities such as the ones listed above, not the
tool's standalone usefulness for a single admin auditing the shares they
connect to.
