# Architecture and scope

```
WPF UI (button-activated)
    |
    v
WindowsInventoryScanner (local, read-only)
    |-- Windows uninstall registry keys -> desktop software display names
    |-- Chrome / Edge / Brave public extension package directories -> allowlisted extension IDs
    |
    v
InventoryRecord (non-sensitive metadata only)
    |
    +--> on-screen DataGrid + filter
    +--> explicit local CSV export
```

## Separation of responsibilities

- **SafeDeskInventory.Core**: data model, allowlisted catalog, normalization, CSV escaping, synthetic demo data.
- **SafeDeskInventory.Windows**: Windows-only, read-only adapters for uninstall registry and extension *package directories*.
- **SafeDeskInventory.App**: WPF view and explicit scan/export actions.
- **SafeDeskInventory.Tests**: dependency-free, cross-platform console-based checks of core logic.

## Trust boundaries

- The application never sends data to any service and has no HTTP dependencies.
- A scan requires deliberate user action and a confirmation dialog.
- Only public uninstall metadata and public extension *installation package directory names* are inspected.
- Do **not** inspect browser Local Extension Settings, IndexedDB, LevelDB, preferences, wallet directories or browser credential stores.
- No private keys, seed phrases, passwords, wallet addresses, transactions or balances are retrieved.
- A directory existing on disk does *not* establish that its extension is enabled or in use.
- "Install Date" comes only from Windows uninstall metadata; if unavailable, show "Not available".
- Export is local-only and initiated by the user. CSV formula-like values are escaped.

## Limits of the sample

- This is a deliberately scoped, transparent prototype, not comprehensive endpoint-management software.
- Browser support: Chrome, Edge, Brave (Chromium package directories only).
- Firefox and extension activation-state validation are **not** implemented.
- Browser extension package-version reading is best-effort.
- Install timestamps for browser extensions are not inferred from filesystem dates.
- Portable desktop apps and per-user unconventional installer formats may not appear in Windows uninstall registry.
- The catalog intentionally contains a small number of product names and two public extension IDs; it can be expanded with source review.
- Registry app-name matching can produce false positives; real product work should implement signed publisher identity checks when available.
- The sample includes a test harness but has not been independently built or validated on a Windows computer in this environment.
