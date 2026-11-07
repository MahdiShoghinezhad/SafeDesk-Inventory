namespace SafeDeskInventory.Core.Services;

/// <summary>
/// Public software display names and public Chrome Web Store extension IDs.
/// This catalog is intentionally small and auditable.
/// </summary>
public static class WalletCatalog
{
    private static readonly string[] DesktopNames =
    [
        "Ledger Live", "Trezor Suite", "Exodus", "Electrum",
        "Sparrow Wallet", "Atomic Wallet", "Wasabi Wallet"
    ];

    public static IReadOnlyDictionary<string, string> ChromiumExtensions { get; } =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            // Public, published Chrome Web Store identifiers; never wallet identifiers.
            ["nkbihfbeogaeaoehlefnkodbefgpgknn"] = "MetaMask",
            ["bfnaelmomeimhlpmgjnjophhpkkoljpa"] = "Phantom"
        };

    public static string? MatchDesktopDisplayName(string? displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName)) return null;
        foreach (string product in DesktopNames)
        {
            if (displayName.Contains(product, StringComparison.OrdinalIgnoreCase))
                return product;
        }
        return null;
    }
}
