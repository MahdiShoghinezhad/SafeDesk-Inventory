# Validation plan and release gate

**Owner:** Mahdi Shoghinezhad  
**Status:** Tests and build commands prepared; Windows execution not yet verified as of 2026-11-07.

I use this checklist to separate implementation claims from demonstrated results.

## 1. Source review (static)

- [x] Review software allowlist and public extension package IDs.
- [x] Review that Windows adapter reads uninstall metadata and package-directory names only.
- [x] Review that export is local and initiated by the user.
- [x] Review that the user must explicitly consent before live scanning.
- [x] Confirm there is no network, telemetry, secret-store or scheduled-scanning dependency.

## 2. Automated checks

Run from repository root:

```powershell
dotnet build .\src\SafeDeskInventory.App\SafeDeskInventory.App.csproj -c Release
dotnet run --project .\tests\SafeDeskInventory.Tests\SafeDeskInventory.Tests.csproj -c Release
```

- [ ] Clean build succeeds on Windows.
- [ ] Core smoke checks pass on Windows or supported cross-platform .NET environment.
- [ ] GitHub Actions Windows workflow is green after source publication.
- [ ] Publish creates an artifact that launches successfully on Windows x64.

## 3. Manual Windows GUI acceptance

- [ ] App starts with synthetic-data label and no live scan.
- [ ] Live scan requires a Yes/No confirmation each time.
- [ ] Declining consent leaves current results intact.
- [ ] A clean machine or profile with no matching applications shows zero results without an error.
- [ ] Filtering works for names, browsers and profile labels.
- [ ] Export uses the current filtered view and writes to a user-chosen destination.
- [ ] Formula-like CSV cells are neutralized when opened in spreadsheet software.
- [ ] Missing/denied registry or profile directory access produces understandable warnings.
- [ ] Extension package presence is described only as presence, never activation or ownership.
- [ ] Keyboard navigation and basic UI scaling are manually reviewed.

## 4. Release requirements

I will use the word **verified** only when the relevant checkboxes above are complete and recorded with a date, environment and evidence (for example a public CI run or locally captured test log). Demo data is never evidence of software installed on the test machine.

### Execution log

| Date | Environment | Result | Evidence |
|---|---|---|---|
| 2026-11-07 | Source-only preparation environment (no .NET SDK available) | Static source/XML review only; runtime unverified | Initial repository package |

