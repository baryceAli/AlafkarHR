using SharedWithUI.Inventory.Enums;

namespace SharedWithUI.Inventory.Dtos;

public record StockMovementDto
{

    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid ProductSkuId { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid BatchId { get; set; }
    public Guid? SourceLocationId { get; set; }
    public string? SourceLocationName { get; set; }
    public string? SourceLocationNameEng { get; set; }
    public Guid? DestinationLocationId { get; set; }
    public string? DestinationLocationName { get; set; }
    public string? DestinationLocationNameEng { get; set; }
    public MovementType MovementType { get; set; }

    public string ReferenceNumber { get; set; }
    public string SourceDocumentType { get; set; }
    public Guid? SourceDocumentId { get; set; }
    public Guid? SourceDocumentLineId { get; set; }
    public Guid? ParentProductSkuId { get; set; }
    public Guid? ParentSalesOrderLineId { get; set; }

    public decimal QuantityBefore { get; set; }

    public decimal QuantityAfter { get; set; }
    //public decimal Quantity { get; set; }
    public Guid? ProductPackageId { get; set; }
    public Guid? UnitId { get; set; }
    public decimal EnteredQuantity { get; set; }
    public decimal PackageMultiplier { get; set; }
    public decimal UnitMultiplier { get; set; }
    public decimal NormalizedQuantity { get; set; }
    public decimal ReservedBefore { get; set; }
    public decimal ReservedAfter { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalCost { get; set; }
    public Guid CurrencyId { get; set; }

    public string Notes { get; set; } = string.Empty;
    public MovementDirection MovementDirection { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    //public MovementCategory MovementCategory { get; set; }

}

public class StockMovementFilterDto
{
    public Guid CompanyId { get; set; }
    public Guid? BranchId { get; set; }
    public Guid? WarehouseId { get; set; }
    public Guid? ProductSkuId { get; set; }
    public Guid? BatchId { get; set; }
    public Guid? SourceLocationId { get; set; }
    public Guid? DestinationLocationId { get; set; }
    public string? SourceDocumentType { get; set; }
    public Guid? SourceDocumentId { get; set; }
    public string? ReferenceNumber { get; set; }
    public Guid? ParentProductSkuId { get; set; }
    public bool? ExpiredOnly { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
