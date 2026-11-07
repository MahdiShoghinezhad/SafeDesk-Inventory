using System.IO;
using System.Security;
using Microsoft.Win32;
using SafeDeskInventory.Core.Models;
using SafeDeskInventory.Core.Services;

namespace SafeDeskInventory.Windows;

public sealed record ScanReport(
    IReadOnlyList<InventoryRecord> Records,
    IReadOnlyList<string> Warnings);

/// <summary>
/// Inventory-only implementation. Never opens a wallet, reads an extension database,
/// visits a private wallet directory or transmits any data.
/// </summary>
public sealed class WindowsInventoryScanner
{
    private const string UninstallKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

    public ScanReport Scan()
    {
        var entries = new List<InventoryRecord>();
        var warnings = new HashSet<string>(StringComparer.Ordinal);
        ReadInstalledApps(entries, warnings);
        ReadPublicExtensionPackageFolders(entries, warnings);
        return new ScanReport(InventoryTools.DistinctOrdered(entries), warnings.ToArray());
    }

    private static void ReadInstalledApps(List<InventoryRecord> entries, HashSet<string> warnings)
    {
        foreach (RegistryHive hive in new[] { RegistryHive.LocalMachine, RegistryHive.CurrentUser })
        {
            foreach (RegistryView view in new[] { RegistryView.Registry64, RegistryView.Registry32 })
            {
                try
                {
                    using RegistryKey root = RegistryKey.OpenBaseKey(hive, view);
                    using RegistryKey? uninstall = root.OpenSubKey(UninstallKey);
                    if (uninstall is null) continue;

                    foreach (string name in uninstall.GetSubKeyNames())
                    {
                        try
                        {
                            using RegistryKey? app = uninstall.OpenSubKey(name);
                            if (app is null) continue;
                            string? displayName = app.GetValue("DisplayName") as string;
                            string? wallet = WalletCatalog.MatchDesktopDisplayName(displayName);
                            if (wallet is null) continue;

                            string version = (app.GetValue("DisplayVersion") as string)?.Trim() ?? InventoryTools.Unknown;
                            string date = InventoryTools.NormalizeInstallDate(app.GetValue("InstallDate") as string);
                            entries.Add(new InventoryRecord(wallet, "Desktop application", "Windows uninstall registry",
                                "—", "—", version.Length == 0 ? InventoryTools.Unknown : version, date));
                        }
                        catch (Exception ex) when (ex is SecurityException or UnauthorizedAccessException or IOException)
                        {
                            warnings.Add("Some installed-application records were not readable.");
                        }
                    }
                }
                catch (Exception ex) when (ex is SecurityException or UnauthorizedAccessException or IOException)
                {
                    warnings.Add("One or more Windows uninstall registry views were unavailable.");
                }
            }
        }
    }

    private static void ReadPublicExtensionPackageFolders(List<InventoryRecord> entries, HashSet<string> warnings)
    {
        string local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        if (string.IsNullOrWhiteSpace(local)) return;

        var browsers = new (string Name, string Root)[]
        {
            ("Chrome", Path.Combine(local, "Google", "Chrome", "User Data")),
            ("Edge", Path.Combine(local, "Microsoft", "Edge", "User Data")),
            ("Brave", Path.Combine(local, "BraveSoftware", "Brave-Browser", "User Data"))
        };

        foreach (var browser in browsers)
        {
            if (!Directory.Exists(browser.Root)) continue;
            try
            {
                foreach (string profile in Directory.EnumerateDirectories(browser.Root))
                {
                    string profileName = Path.GetFileName(profile);
                    // Only known profile directory patterns; never enumerate arbitrary nested files.
                    if (profileName != "Default" &&
                        !(profileName.StartsWith("Profile ", StringComparison.Ordinal) &&
                          int.TryParse(profileName[8..], out _)))
                        continue;

                    foreach (var extension in WalletCatalog.ChromiumExtensions)
                    {
                        string packageRoot = Path.Combine(profile, "Extensions", extension.Key);
                        if (!Directory.Exists(packageRoot)) continue;

                        // Merely inspect published package directory names (versions).
                        // Deliberately do NOT read manifest, preferences, local storage, IndexedDB or LevelDB.
                        string version = InventoryTools.Unknown;
                        try
                        {
                            var versions = Directory.EnumerateDirectories(packageRoot)
                                .Select(Path.GetFileName)
                                .Where(v => !string.IsNullOrWhiteSpace(v) &&
                                            v!.All(c => char.IsDigit(c) || c == '.' || c == '_'))
                                .OrderByDescending(v => v, StringComparer.Ordinal)
                                .ToArray();
                            if (versions.Length > 0) version = versions[0]!;
                        }
                        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or SecurityException)
                        {
                            warnings.Add("An extension package version could not be read.");
                        }

                        entries.Add(new InventoryRecord(extension.Value, "Browser extension",
                            "Public extension package folder", browser.Name, profileName,
                            version, InventoryTools.Unknown));
                    }
                }
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or SecurityException)
            {
                warnings.Add("Some browser extension package directories were not accessible.");
            }
        }
    }
}
