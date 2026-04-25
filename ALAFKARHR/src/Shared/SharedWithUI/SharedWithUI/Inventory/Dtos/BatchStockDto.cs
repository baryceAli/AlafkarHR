namespace SharedWithUI.Inventory.Dtos;

/// <summary>
/// DTO for BatchStock (quantity tracking per batch)
/// </summary>
public record BatchStockDto(
    Guid Id,
    Guid BatchId,
    decimal Quantity,
    decimal ReservedQuantity,
    decimal Available,
    DateTime CreatedAt,
    string CreatedBy,
    DateTime? LastModified,
    string? LastModifiedBy,
    DateTime? DeletedAt,
    string? DeletedBy
);


public record CreateBatchStockDto(
    Guid Id,
    Guid BatchId,
    decimal Quantity
    //decimal ReservedQuantity,
    //decimal Available
);
public record RemoveBatchStockDto(
    Guid InventoryId,
    Guid BatchId,
    string Notes);

