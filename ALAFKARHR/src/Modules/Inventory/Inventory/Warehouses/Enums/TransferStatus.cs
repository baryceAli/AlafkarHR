namespace Inventory.Warehouses.Enums;

public enum TransferStatus
{
    Pending,        // Created but not shipped
    Shipped,        // Left warehouse A
    PartiallyReceived,// Received by warehouse B
    Completed,
    Cancelled
}