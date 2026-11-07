using SafeDeskInventory.Core.Models;

namespace SafeDeskInventory.Core.Services;

public static class DemoData
{
    /// <summary>Synthetic illustrations; not observations of this computer.</summary>
    public static IReadOnlyList<InventoryRecord> Create() =>
    [
        new("Ledger Live", "Desktop application", "DEMO / synthetic", "—", "—", "2.12.0", "2026-10-10"),
        new("Trezor Suite", "Desktop application", "DEMO / synthetic", "—", "—", "24.5.1", "2026-10-12"),
        new("MetaMask", "Browser extension", "DEMO / synthetic", "Chrome", "Default", "13.0.0", "Not available"),
        new("Phantom", "Browser extension", "DEMO / synthetic", "Edge", "Profile 1", "26.0.0", "Not available")
    ];
}
