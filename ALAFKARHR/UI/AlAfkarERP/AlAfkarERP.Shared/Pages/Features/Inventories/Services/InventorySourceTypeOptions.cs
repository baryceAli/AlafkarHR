namespace AlAfkarERP.Shared.Pages.Features.Inventories.Services;

public sealed record InventorySourceTypeOption(string Value, string LabelEn, string LabelAr);

public static class InventorySourceTypeOptions
{
    public const string StockIn = "InventoryStockIn";
    public const string OpeningBalance = "OpeningBalance";
    public const string ManualReceipt = "ManualReceipt";
    public const string StockOut = "InventoryStockOut";
    public const string ManualIssue = "ManualIssue";
    public const string DamageWriteOff = "DamageWriteOff";
    public const string Adjustment = "InventoryAdjustment";
    public const string CycleCount = "CycleCount";
    public const string Correction = "Correction";
    public const string Reservation = "InventoryReservation";
    public const string Release = "InventoryRelease";

    public static readonly IReadOnlyList<InventorySourceTypeOption> StockInOptions =
    [
        new(StockIn, "Inventory Stock In", "مخزون وارد"),
        new(OpeningBalance, "Opening Balance", "رصيد افتتاحي"),
        new(ManualReceipt, "Manual Receipt", "استلام يدوي")
    ];

    public static readonly IReadOnlyList<InventorySourceTypeOption> StockOutOptions =
    [
        new(StockOut, "Inventory Stock Out", "مخزون صادر"),
        new(ManualIssue, "Manual Issue", "صرف يدوي"),
        new(DamageWriteOff, "Damage Write-off", "إتلاف مخزون")
    ];

    public static readonly IReadOnlyList<InventorySourceTypeOption> AdjustmentOptions =
    [
        new(Adjustment, "Inventory Adjustment", "تسوية مخزون"),
        new(CycleCount, "Cycle Count", "جرد دوري"),
        new(Correction, "Correction", "تصحيح")
    ];

    public static readonly IReadOnlyList<InventorySourceTypeOption> ReservationOptions =
    [
        new(Reservation, "Inventory Reservation", "حجز مخزون")
    ];

    public static readonly IReadOnlyList<InventorySourceTypeOption> ReleaseOptions =
    [
        new(Release, "Inventory Release", "إطلاق مخزون")
    ];
}
