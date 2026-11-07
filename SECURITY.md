# Security and privacy

SafeDesk Inventory is designed for software asset inventory on devices you own or administer with permission.

## Data read
- Windows uninstall registry: installed-app display name, display version, optional install date.
- Chromium extension *package directory names*: allowlisted public IDs and package version-directory names.

## Data never read
- Seed phrases, private keys, keystores, wallet configuration or wallet balance.
- Browser profile secrets, wallet extension local storage, cookies, IndexedDB, LevelDB or session files.
- Wallet processes, memory contents, addresses, identities, account records or transaction histories.

## Communications
There is no networking code, analytics or telemetry. The user must manually consent to scanning and manually select a destination to export a CSV. No background service, persistence or automatic scheduling is used.

## Reporting issues
Please open a GitHub issue without attaching private device inventories or sensitive user information. For a security vulnerability, use GitHub's private vulnerability reporting if enabled.
