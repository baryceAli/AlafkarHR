namespace SharedWithUI.Inventory.Enums;
/// <summary>
/// Enum for stock movement types in the Inventory module
/// </summary>
public enum MovementType
{
    OpeningBalance=0,
    TransferIn=1,
    TransferOut=2,
    PurchaseReceipt=3,
    SalesShipment=4,
    CustomerReturn=5,
    SupplierReturn=6,
    AdjustmentIncrease=7,
    AdjustmentDecrease=8,
    ProductionIn=9,
    ProductionOut=10,
    ReserveAmount=11,
    ReleaseAmount=12,
    Scrap=13
}
