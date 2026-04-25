namespace Inventory.Warehouses.Enums;

public enum TransferStatus
{
    Pending,        // Created but not shipped
    Shipped,        // Left warehouse A
    Completed,      // Received by warehouse B
    Cancelled
}