# SafeDesk Inventory

**A privacy-first Windows desktop inventory prototype — C# · WPF · .NET 10**

[![Windows build and smoke tests](https://github.com/MahdiShoghinezhad/SafeDesk-Inventory/actions/workflows/windows-build.yml/badge.svg)](https://github.com/MahdiShoghinezhad/SafeDesk-Inventory/actions/workflows/windows-build.yml)
![Platform](https://img.shields.io/badge/platform-Windows%2010%20%2F%2011-informational)
![License](https://img.shields.io/badge/license-MIT-blue)

**Designed and maintained by [Mahdi Shoghinezhad](https://github.com/MahdiShoghinezhad).**

> **Project status: portfolio prototype — Windows CI build, core smoke checks, and publish passed; manual GUI acceptance remains outstanding.** I distinguish implemented features from tested behavior below. Sample data in this repository is synthetic, not evidence of anyone's installed wallets.

## Why I built it

I build software with an emphasis on clear requirements, testability, maintainability and thoughtful security boundaries. SafeDesk Inventory is a focused desktop case study: how can an application report the presence of *known software packages* without crossing into users' private application data?

My answer is a deliberately narrow, consent-first inventory workflow. The app reads a small allowlist of Windows uninstall metadata and checks the presence of known browser-extension **package directories**. It does not open wallets, inspect account storage or contact a remote service.

This is a reusable foundation for desktop inventory, endpoint-support utilities and secure local reporting. It is **not** a cryptocurrency wallet recovery, key-discovery, forensic or balance-analysis product.

## Engineering highlights

| Area | My implementation approach | Why it matters |
|---|---|---|
| Local discovery | Read-only Windows uninstall-registry metadata and allowlisted Chromium package-folder names | Narrow, reviewable scope and no privileged wallet access |
| User consent | A confirmation dialog before every live scan; demo data shown by default | Avoids unexpected collection |
| Architectural boundaries | Separate UI, core models/export, and Windows adapters | Keeps business logic independently testable |
| Results | Searchable WPF grid, per-type counts, clear live/demo indicators | Makes the output usable and unambiguous |
| Reporting | User-initiated local CSV export with formula-injection mitigation | Reduces spreadsheet risk and avoids hidden data transmission |
| Verification | Dependency-free core smoke checks and a Windows CI workflow | Makes expected behavior inspectable and repeatable |

## What the prototype detects

**Desktop:** a short reviewed allowlist of application display names (e.g., Ledger Live, Trezor Suite, Exodus, Electrum, Sparrow, Atomic Wallet, Wasabi Wallet) from Windows uninstall entries. A matching name is an *installation observation*, not verified publisher identity.

**Browser:** public package-folder presence for two published extension IDs (MetaMask and Phantom) under standard Chrome, Edge and Brave user-profile locations. A folder can remain after removal; its existence **does not prove** that an extension is enabled, in use or associated with a wallet account.

| Data | Included? | Caveat |
|---|---:|---|
| App / extension display name | Yes | Allowlisted matches only |
| Browser / profile directory label | Yes | Basic standard profiles only |
| Version | Best effort | May be unavailable or stale |
| Desktop install date | Best effort | Often absent in Windows registry |
| Extension enabled state | No | Presence-only detection |
| Firefox support | No | Not implemented |
| Keys, seed phrases, addresses, balances or transactions | **Never** | Out of scope by design |

## Architecture

```text
             WPF interface (explicit user action)
                          |
             WindowsInventoryScanner
                     /           \
     Windows uninstall keys     Browser package-folder names
                     \           /
                      InventoryRecord
                             |
           Core: normalization · deduplication · CSV
                             |
            Local searchable grid / opt-in export
```

```text
src/
  SafeDeskInventory.App/        WPF app and interaction layer
  SafeDeskInventory.Core/       Models, allowlist, normalization, CSV
  SafeDeskInventory.Windows/    Read-only Windows discovery adapter
tests/
  SafeDeskInventory.Tests/      Dependency-free core smoke checks
docs/
  ARCHITECTURE.md              Trust boundaries and limitations
  DESIGN_DECISIONS.md          My engineering tradeoffs
  VALIDATION.md                How I verify the deliverable
  DEMO_SCRIPT.md               Short portfolio walkthrough
```

## Build and run (Windows 10/11)

**Prerequisite:** [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0). Visual Studio with the .NET desktop workload is optional.

```powershell
dotnet --version
dotnet build .\src\SafeDeskInventory.App\SafeDeskInventory.App.csproj -c Release
dotnet run --project .\tests\SafeDeskInventory.Tests\SafeDeskInventory.Tests.csproj
dotnet run --project .\src\SafeDeskInventory.App\SafeDeskInventory.App.csproj
```

The app opens with **DEMO — SYNTHETIC DATA**. Select **Load demo** for a reproducible walkthrough. Use **Scan this PC** only on a device you own or administer, and only after reviewing the consent dialog. Zero matching results is a valid result.

To create a self-contained Windows x64 build:

```powershell
dotnet publish .\src\SafeDeskInventory.App\SafeDeskInventory.App.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o .\publish\win-x64
```

See [validation checklist](docs/VALIDATION.md). **CI compilation is not the same as a verified interactive Windows run.** I will not label the binary production-tested before that checklist is completed.

## Example report (synthetic)

```csv
Name,Type,Source,Browser,Profile,Version,Install Date
"Ledger Live","Desktop application","DEMO / synthetic","—","—","2.12.0","2026-10-10"
"MetaMask","Browser extension","DEMO / synthetic","Chrome","Default","13.0.0","Not available"
```

These are **fabricated demo observations** and should never be represented as scan evidence.

## Security & privacy commitment

I intentionally designed this project around data minimization. No networking library, analytics SDK, scheduled scanning or background service is used. The app does not read browser extension preferences, cookies, wallet storage, IndexedDB, LevelDB, private keys or seed phrases. It never claims to identify a person's wallet accounts.

For the exact trust boundary, review [SECURITY.md](SECURITY.md) and [architecture](docs/ARCHITECTURE.md). For specific technical choices and areas I would improve in a production contract, see [design decisions](docs/DESIGN_DECISIONS.md).

## Roadmap and honest limitations

- [x] Desktop UI and synthetic demonstration data
- [x] Registry-backed allowlisted software observation
- [x] Chromium package-presence observation (Chrome / Edge / Brave)
- [x] Local opt-in CSV export and dependency-free core checks
- [x] Windows CI workflow definition
- [x] Verified passing Windows CI build, core smoke checks and self-contained publish ([run #1](https://github.com/MahdiShoghinezhad/SafeDesk-Inventory/actions/runs/42391699676))
- [ ] Manual GUI acceptance test on Windows 10/11
- [ ] Additional automated Windows adapter tests with controlled fixtures
- [ ] Verified publisher identity and more robust installation-source handling
- [ ] Accessibility and screen-reader review
- [ ] Optional expanded application catalogs (without reading account data)

## Working with me

I value explicit acceptance criteria, small reviewable milestones and code that a client can inspect and maintain. This repository demonstrates my approach to separating responsibilities, documenting constraints and engineering for privacy. For private contract work, I define supported platforms, exact inventory fields and a test matrix before committing to feature parity.

**Author:** Mahdi Shoghinezhad · [GitHub](https://github.com/MahdiShoghinezhad)  
**License:** [MIT](LICENSE) · Copyright © 2026 Mahdi Shoghinezhad
