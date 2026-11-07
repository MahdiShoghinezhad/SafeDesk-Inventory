# My 2-minute portfolio walkthrough

**Presenter: Mahdi Shoghinezhad**

> I built SafeDesk Inventory to demonstrate a Windows desktop inventory workflow that stays on the user's machine. The most important design choice isn't what it collects — it's what it deliberately refuses to collect.

1. **Start with provenance.** Open the application and point out `DEMO — SYNTHETIC DATA`. I do not present fabricated records as evidence from a real device.
2. **Show practical UI.** Search for `MetaMask` or `Ledger`, review separate inventory counts, and filter the grid.
3. **Demonstrate safe export.** Export to a local CSV, explain quoting and spreadsheet formula-injection mitigation.
4. **Show consent.** Select `Scan this PC` on an authorized device and explain the confirmation dialog. A zero-row result is valid.
5. **Explain the boundary.** I inspect uninstall registry entries and known extension *package folders*, never wallet storage, credentials or balances.
6. **Show maintainability.** Open the three project layers, the `DESIGN_DECISIONS.md` document, the smoke tests and the Windows build workflow.
7. **Close with precision.** Folder presence is not an enabled-state check; registry install dates can be missing. I document these limits rather than guessing.

**Do not claim Windows runtime success before the CI build and manual Windows checklist pass.**
