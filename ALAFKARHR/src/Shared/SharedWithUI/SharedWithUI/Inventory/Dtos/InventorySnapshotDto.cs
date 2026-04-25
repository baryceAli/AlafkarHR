namespace SharedWithUI.Inventory.Dtos;

public record InventorySnapshotDto(
    Guid Id,
    Guid ProductId,
    Guid ProductSkuId,
    Guid WarehouseId,
    Guid BatchId,
    double Quantity,
    double ReservedQuantity,
    byte[] RowVersion,
    DateTime LastUpdated
);
