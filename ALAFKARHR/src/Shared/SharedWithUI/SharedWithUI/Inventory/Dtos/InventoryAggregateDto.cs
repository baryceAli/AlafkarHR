using SharedWithUI.Inventory.Enums;
using System.ComponentModel.DataAnnotations;

namespace SharedWithUI.Inventory.Dtos;

/// <summary>
/// DTO for displaying Inventory with all batch stock information
/// </summary>
public class InventoryAggregateDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? ProductNameEng { get; set; }
    public Guid ProductSkuId { get; set; }
    public string? ProductSkuName { get; set; }
    public string? ProductSkuNameEng { get; set; }
    public Guid WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
    public string? WarehouseNameEng { get; set; }
    public Guid? BranchId { get; set; }
    public string? BranchName { get; set; }
    public string? BranchNameEng { get; set; }
    public decimal TotalQuantity { get; set; }
    public decimal TotalReserved { get; set; }
    public decimal TotalAvailable { get; set; }
    //public decimal TotalQuantity => _batches.Sum(x => x.Quantity);
    //public decimal TotalReserved => _batches.Sum(x => x.ReservedQuantity);
    //public decimal TotalAvailable => TotalQuantity - TotalReserved;

    public List<BatchStockDto> Batches { get; set; }
    public Guid CompanyId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
    
}




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

    public Guid? ProductPackageId { get; set; }
    public Guid? UnitId { get; set; }
    public string? UnitName { get; set; }
    public string? UnitNameEng { get; set; }
    public decimal UnitMultiplier { get; set; } = 1;
    public decimal NormalizedQuantity { get; set; }

    [Required(ErrorMessage = "Warehouse is required")]
    public Guid? WarehouseId { get; set; }


    [Required(ErrorMessage = "Batch is required")]
    public Guid InitialBatchId { get; set; }


    [Required(ErrorMessage = "Quantity is required")]
    //[Range(0.01,100000,ErrorMessage = "Quantity must be greator than 0")]
    public decimal InitialQuantity { get; set; }
    public MovementType MovementType { get; set; }
    
    
    [Required(ErrorMessage ="Cost is required")]    
    public decimal UnitCost { get; set; }


    [Required(ErrorMessage ="")]
    public decimal TotalCost { get; set; }

    [Required(ErrorMessage ="Currency is required")]
    public Guid? CurrencyId { get; set; }

    [Obsolete("Use CurrencyId instead.")]
    public Guid? Currency
    {
        get => CurrencyId;
        set => CurrencyId = value;
    }

    public Guid CompanyId { get; set; }
    public string? Notes { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? SourceDocumentType { get; set; }
    public Guid? SourceDocumentId { get; set; }
    public Guid? SourceDocumentLineId { get; set; }
    public Guid? ParentProductSkuId { get; set; }
    public Guid? ParentSalesOrderLineId { get; set; }
    public Guid? SourceLocationId { get; set; }
    public Guid? DestinationLocationId { get; set; }
    public bool ConsumeReservedQuantity { get; set; }
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

