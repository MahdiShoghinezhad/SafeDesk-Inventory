using System.Text;
using SafeDeskInventory.Core.Models;

namespace SafeDeskInventory.Core.Services;

public static class CsvExporter
{
    public static string Export(IEnumerable<InventoryRecord> records)
    {
        var csv = new StringBuilder();
        csv.AppendLine("Name,Type,Source,Browser,Profile,Version,Install Date");
        foreach (var r in records)
        {
            csv.AppendLine(string.Join(",", new[]
            {
                Escape(r.Name), Escape(r.Kind), Escape(r.Source), Escape(r.Browser),
                Escape(r.Profile), Escape(r.Version), Escape(r.InstallDate)
            }));
        }
        return csv.ToString();
    }

    private static string Escape(string? value)
    {
        string safe = value ?? "";
        // Prevent CSV formula injection when a report is opened in a spreadsheet.
        // Leading whitespace can disguise a spreadsheet formula in exported values.
        string candidate = safe.TrimStart();
        if (candidate.Length > 0 && "=+-@".Contains(candidate[0])) safe = "'" + safe;
        if (safe.Length > 0 && "\t\r\n".Contains(safe[0])) safe = "'" + safe;
        return "\"" + safe.Replace("\"", "\"\"") + "\"";
    }
}
