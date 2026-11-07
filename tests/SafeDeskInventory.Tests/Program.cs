using SafeDeskInventory.Core.Models;
using SafeDeskInventory.Core.Services;

int passed = 0;
void Check(bool condition, string name)
{
    if (!condition) throw new Exception($"FAIL: {name}");
    Console.WriteLine($"PASS: {name}");
    passed++;
}

Check(WalletCatalog.MatchDesktopDisplayName("Ledger Live 2.4") == "Ledger Live", "Known app name");
Check(WalletCatalog.MatchDesktopDisplayName("Unrelated printer driver") is null, "Unknown app is excluded");
Check(WalletCatalog.ChromiumExtensions["nkbihfbeogaeaoehlefnkodbefgpgknn"] == "MetaMask", "Public extension ID");
Check(InventoryTools.NormalizeInstallDate("20261107") == "2026-11-07", "Registry date parser");
Check(InventoryTools.NormalizeInstallDate("invalid") == InventoryTools.Unknown, "Invalid date stays unknown");
var demo = DemoData.Create();
Check(demo.Count == 4 && demo.All(x => x.Source.Contains("DEMO")), "Demo data clearly labeled");
var same = new InventoryRecord("MetaMask", "Browser extension", "Package folder", "Chrome", "Default", "1.0", "Not available");
var otherProfile = same with { Profile = "Profile 1" };
Check(InventoryTools.DistinctOrdered(new[] { same, same, otherProfile }).Count == 2,
      "Duplicates dropped but separate profiles retained");
var dangerous = same with { Name = "=SUM(1,2)" };
Check(CsvExporter.Export(new[] { dangerous }).Contains("\"'=SUM(1,2)\""), "CSV formula cells neutralized");
var paddedFormula = same with { Name = "  +SUM(1,2)" };
Check(CsvExporter.Export(new[] { paddedFormula }).Contains("\"'  +SUM(1,2)\""),
      "Leading whitespace does not bypass formula neutralization");
var quoted = same with { Name = "A \"quoted\" app" };
Check(CsvExporter.Export(new[] { quoted }).Contains("\"A \"\"quoted\"\" app\""),
      "CSV double-quote escaping");
Check(CsvExporter.Export(new[] { same }).StartsWith("Name,Type,Source"), "CSV header is stable");
Console.WriteLine($"\n{passed} checks passed.");
