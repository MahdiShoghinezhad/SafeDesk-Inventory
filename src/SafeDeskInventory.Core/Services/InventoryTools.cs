using System.Globalization;
using SafeDeskInventory.Core.Models;

namespace SafeDeskInventory.Core.Services;

public static class InventoryTools
{
    public const string Unknown = "Not available";

    /// <summary>Normalize optional YYYYMMDD data found in Windows uninstall keys.</summary>
    public static string NormalizeInstallDate(string? value)
    {
        if (!DateTime.TryParseExact(value, "yyyyMMdd", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out DateTime date))
            return Unknown;
        return date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
    }

    /// <summary>Only identical observations are deduplicated; browser profiles stay separate.</summary>
    public static IReadOnlyList<InventoryRecord> DistinctOrdered(IEnumerable<InventoryRecord> records) =>
        records.DistinctBy(r => (r.Name.ToUpperInvariant(), r.Kind, r.Browser,
                                r.Profile, r.Version, r.Source))
               .OrderBy(r => r.Kind, StringComparer.OrdinalIgnoreCase)
               .ThenBy(r => r.Name, StringComparer.OrdinalIgnoreCase)
               .ThenBy(r => r.Browser, StringComparer.OrdinalIgnoreCase)
               .ThenBy(r => r.Profile, StringComparer.OrdinalIgnoreCase)
               .ToArray();
}
