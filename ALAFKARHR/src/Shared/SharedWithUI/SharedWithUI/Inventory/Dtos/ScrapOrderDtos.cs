namespace SharedWithUI.Inventory.Dtos;

public class ScrapOrderFilterDto
{
    public Guid CompanyId { get; set; }
    public Guid? BranchId { get; set; }
    public Guid? WarehouseId { get; set; }
    public ScrapOrderStatus? Status { get; set; }
    public string? SourceDocumentType { get; set; }
    public Guid? SourceDocumentId { get; set; }
}

public class CreateScrapOrderDto
{
    public Guid CompanyId { get; set; }
    public Guid? BranchId { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid? SourceLocationId { get; set; }
    public Guid? ScrapLocationId { get; set; }
    public string? SourceDocumentType { get; set; }
    public Guid? SourceDocumentId { get; set; }
    public Guid? SourceDocumentLineId { get; set; }
    public string? SourceDocumentNumber { get; set; }
    public Guid? SourceInventoryOperationId { get; set; }
    public Guid? SourceInventoryOperationLineId { get; set; }
    public bool ReplenishQuantity { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
    public List<CreateScrapOrderLineDto> Lines { get; set; } = [];
}

public class CreateScrapOrderLineDto
{
    public Guid ProductId { get; set; }
    public Guid ProductSkuId { get; set; }
    public Guid? ProductPackageId { get; set; }
    public Guid? UnitId { get; set; }
    public Guid BatchId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalCost { get; set; }
    public Guid CurrencyId { get; set; }
    public Guid? SourceLocationId { get; set; }
    public Guid? ScrapLocationId { get; set; }
    public Guid? SourceDocumentLineId { get; set; }
    public Guid? SourceInventoryOperationLineId { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
    public List<InventorySerialSelectionDto> SerialNumbers { get; set; } = [];
}

public class ValidateScrapOrderDto
{
    public bool Confirm { get; set; } = true;
}

public class ScrapOrderDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid? BranchId { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid? SourceLocationId { get; set; }
    public Guid ScrapLocationId { get; set; }
    public string? SourceDocumentType { get; set; }
    public Guid? SourceDocumentId { get; set; }
    public Guid? SourceDocumentLineId { get; set; }
    public string? SourceDocumentNumber { get; set; }
    public Guid? SourceInventoryOperationId { get; set; }
    public Guid? SourceInventoryOperationLineId { get; set; }
    public string ScrapOrderNumber { get; set; } = string.Empty;
    public ScrapOrderStatus Status { get; set; }
    public bool ReplenishQuantity { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
    public DateTime? ValidatedAt { get; set; }
    public string? ValidatedBy { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancelledBy { get; set; }
    public decimal TotalQuantity { get; set; }
    public decimal TotalCost { get; set; }
    public List<ScrapOrderLineDto> Lines { get; set; } = [];
}

public class ScrapOrderLineDto
{
    public Guid Id { get; set; }
    public Guid ScrapOrderId { get; set; }
    public int LineNumber { get; set; }
    public Guid ProductId { get; set; }
    public Guid ProductSkuId { get; set; }
    public Guid? ProductPackageId { get; set; }
    public Guid? UnitId { get; set; }
    public Guid BatchId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalCost { get; set; }
    public Guid CurrencyId { get; set; }
    public Guid? SourceLocationId { get; set; }
    public Guid? ScrapLocationId { get; set; }
    public Guid? SourceDocumentLineId { get; set; }
    public Guid? SourceInventoryOperationLineId { get; set; }
    public Guid? StockMovementId { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
    public List<InventorySerialSelectionDto> SerialNumbers { get; set; } = [];
}

public enum ScrapOrderStatus
{
    Draft = 1,
    Validated = 2,
    Cancelled = 3
}
