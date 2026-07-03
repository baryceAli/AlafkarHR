namespace Inventory.Warehouses.Features.Inventories;

internal static class InventorySourceDocumentTypes
{
    public const string InventoryStockIn = "InventoryStockIn";
    public const string OpeningBalance = "OpeningBalance";
    public const string ManualReceipt = "ManualReceipt";
    public const string InventoryStockOut = "InventoryStockOut";
    public const string ManualIssue = "ManualIssue";
    public const string DamageWriteOff = "DamageWriteOff";
    public const string InventoryAdjustment = "InventoryAdjustment";
    public const string CycleCount = "CycleCount";
    public const string Correction = "Correction";
    public const string InventoryReservation = "InventoryReservation";
    public const string InventoryRelease = "InventoryRelease";
    public const string SalesOrderReservation = "SalesOrderReservation";
    public const string SalesOrderReservationRelease = "SalesOrderReservationRelease";

    private static readonly HashSet<string> Allowed = new(StringComparer.OrdinalIgnoreCase)
    {
        InventoryStockIn,
        OpeningBalance,
        ManualReceipt,
        InventoryStockOut,
        ManualIssue,
        DamageWriteOff,
        InventoryAdjustment,
        CycleCount,
        Correction,
        InventoryReservation,
        InventoryRelease,
        SalesOrderReservation,
        SalesOrderReservationRelease,
        "Integration",
        "PurchaseReceipt",
        "SupplierReturn",
        "SalesDeliveryNote",
        "SalesReturn",
        "POSDirectSale"
    };

    public static bool IsAllowed(string? sourceDocumentType) =>
        !string.IsNullOrWhiteSpace(sourceDocumentType)
        && Allowed.Contains(sourceDocumentType);
}
