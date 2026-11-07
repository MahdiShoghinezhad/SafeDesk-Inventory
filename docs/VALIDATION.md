# Validation plan and release gate

**Owner:** Mahdi Shoghinezhad  
**Status:** Windows CI build, core smoke tests, publish, and user-provided GUI demo/CSV output observed on 2026-11-07. Full manual acceptance remains outstanding.

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

- [x] Clean Release build succeeds on Windows GitHub Actions (2026-11-07).
- [x] Core smoke checks pass on Windows GitHub Actions (2026-11-07).
- [x] GitHub Actions Windows workflow is green after source publication ([successful run](https://github.com/MahdiShoghinezhad/SafeDesk-Inventory/actions/runs/42391911145)).
- [x] Publish creates a self-contained Windows x64 artifact (CI).
- [x] User-provided screenshot demonstrates the WPF GUI opened on Windows (synthetic demo only).
- [x] User-provided CSV confirms the four demo observations were exported correctly.
- [ ] Live inventory scan and complete Windows GUI acceptance verified across controlled test fixtures.

## 3. Manual Windows GUI acceptance

- [x] A supplied GUI screenshot shows synthetic demo labeling and four displayed records (does not independently establish that no previous live scan occurred).
- [ ] Live scan requires a Yes/No confirmation each time.
- [ ] Declining consent leaves current results intact.
- [ ] A clean machine or profile with no matching applications shows zero results without an error.
- [ ] Filtering works for names, browsers and profile labels.
- [x] User-provided CSV demonstrates successful export of all four demo observations.
- [ ] Confirm filtered-view CSV export and user-selected destination through a separate manual test.
- [ ] Formula-like CSV cells are neutralized when opened in spreadsheet software.
- [ ] Missing/denied registry or profile directory access produces understandable warnings.
- [ ] Extension package presence is described only as presence, never activation or ownership.
- [ ] Keyboard navigation and basic UI scaling are manually reviewed.

## 4. Release requirements

I will use the word **verified** only when the relevant checkboxes above are complete and recorded with a date, environment and evidence (for example a public CI run or locally captured test log). Demo data is never evidence of software installed on the test machine.

### Execution log

| Date | Environment | Result | Evidence |
|---|---|---|---|
| 2026-11-07 | Source-only preparation environment (no .NET SDK available) | Static source/XML review | Initial repository package |
| 2026-11-07 | GitHub Actions / windows-latest | Release build, core smoke checks, self-contained publish: PASS | [CI run #2](https://github.com/MahdiShoghinezhad/SafeDesk-Inventory/actions/runs/42391911145) |
| 2026-11-07 | User-provided Windows screenshot and exported CSV | WPF demo visibly launched; four synthetic records exported | [Checked-in sanitized demo CSV](../examples/safedesk-demo.inventory.csv); screenshot retained by owner, not yet published |
| Pending | Consent-based Windows test computer | Registry and extension discovery, permission failures, filtered exports, GUI interactions | Full manual acceptance checklist above |

