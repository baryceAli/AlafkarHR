namespace SharedWithUI.Inventory.Dtos;

/// <summary>
/// DTO for displaying Inventory with all batch stock information
/// </summary>
public record InventoryAggregateDto(
    Guid Id,
    Guid ProductId,
    Guid ProductSkuId,
    Guid WarehouseId,
    decimal TotalQuantity,
    decimal TotalReserved,
    decimal TotalAvailable,
    List<BatchStockDto> Batches,
    DateTime CreatedAt,
    string CreatedBy,
    DateTime? LastModified,
    string? LastModifiedBy
);


/// <summary>
/// Simplified DTO for inventory display (without detailed batch info)
/// </summary>
public record InventoryAggregateSummaryDto(
    Guid Id,
    Guid ProductSkuId,
    Guid WarehouseId,
    decimal TotalQuantity,
    decimal TotalReserved,
    decimal TotalAvailable,
    int BatchCount
);

/// <summary>
/// DTO for creating initial inventory
/// </summary>
public record CreateInventoryAggregateDto(
    Guid ProductId,
    Guid ProductSkuId,
    Guid WarehouseId,
    Guid InitialBatchId,
    decimal InitialQuantity
);


public record ReleaseQuantityDto(
    Guid InventoryId,
    Guid ProductSkuId,
    Guid WarehouseId,
    Guid BatchId, 
    decimal quantity);

public record ReserveQuantityDto(
    Guid InventoryId,
    Guid ProductId,
    Guid ProductSkuId,
    Guid WarehouseId,
    Guid batchId,
    decimal quantity);

