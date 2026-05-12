namespace SharedWithUI.Inventory.Enums;
/// <summary>
/// Enum for stock movement types in the Inventory module
/// </summary>
public enum MovementType
{
    PurchaseReceipt,
    SalesShipment,
    TransferIn,
    TransferOut,
    CustomerReturn,
    SupplierReturn,
    AdjustmentIncrease,
    AdjustmentDecrease,
    ProductionIn,
    ProductionOut,
    OpeningBalance,
    ReserverAmount,
    RelseaseAmount
}
