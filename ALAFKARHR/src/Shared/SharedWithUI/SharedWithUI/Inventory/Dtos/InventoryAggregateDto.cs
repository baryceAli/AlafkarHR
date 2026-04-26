using System.ComponentModel.DataAnnotations;

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
public class CreateInventoryAggregateDto
{
    [Required(ErrorMessage ="Product is required")]
    public Guid? ProductId { get; set; }


    [Required(ErrorMessage = "ProductSku is required")]
    public Guid? ProductSkuId { get; set; }


    [Required(ErrorMessage = "Warehouse is required")]
    public Guid? WarehouseId { get; set; }


    [Required(ErrorMessage = "Batch is required")]
    public Guid InitialBatchId { get; set; }


    [Required(ErrorMessage = "Quantity is required")]
    //[Range(0.01,100000,ErrorMessage = "Quantity must be greator than 0")]
    public decimal InitialQuantity { get; set; }
    
}


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

