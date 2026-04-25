namespace SharedWithUI.Inventory.Dtos;

/// <summary>
/// DTO for displaying Batch information (metadata only)
/// </summary>
public record BatchDto(
    Guid Id,
    //Guid WarehouseId,
    Guid ProductId,
    Guid ProductSkuId,
    string BatchNumber,
    DateTime ManufacturingDate,
    DateTime ExpiryDate,
    DateTime CreatedAt,
    string CreatedBy,
    DateTime? LastModified,
    string? LastModifiedBy,
    DateTime? DeletedAt,
    string? DeletedBy
);

/// <summary>
/// DTO for creating a Batch
/// </summary>
public record CreateBatchDto(
    //Guid WarehouseId,
    Guid ProductId,
    Guid ProductSkuId,
    string BatchNumber,
    DateTime ManufacturingDate,
    DateTime ExpiryDate
);

/// <summary>
/// DTO for updating a Batch
/// </summary>
public record UpdateBatchDto(
    Guid Id,
    string BatchNumber,
    DateTime ManufacturingDate,
    DateTime ExpiryDate
);
