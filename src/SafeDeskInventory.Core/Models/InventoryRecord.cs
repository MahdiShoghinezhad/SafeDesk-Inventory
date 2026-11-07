namespace SafeDeskInventory.Core.Models;

/// <summary>A deliberately minimal, non-sensitive software inventory record.</summary>
public sealed record InventoryRecord(
    string Name,
    string Kind,
    string Source,
    string Browser,
    string Profile,
    string Version,
    string InstallDate);
