namespace SharedWithUI.Inventory.Dtos;

public record StockMovementDto(
    Guid Id,
    Guid ProductSkuId,
    Guid WarehouseId,
    Guid BatchId,
    string MovementType,
    Guid ReferenceId,
    decimal Quantity,
    DateTime MovementDate,
    string Notes,
    DateTime CreatedAt,
    //string Notes,
    string CreatedBy
);
