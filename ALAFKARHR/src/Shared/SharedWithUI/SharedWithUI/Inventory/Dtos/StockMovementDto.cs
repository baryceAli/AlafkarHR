using SharedWithUI.Inventory.Enums;

namespace SharedWithUI.Inventory.Dtos;

public record StockMovementDto
{

    public Guid ProductId { get; set; }
    public Guid ProductSkuId { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid BatchId { get; set; }
    public MovementType MovementType { get; set; }

    public string ReferenceNumber { get; set; }
    public string SourceDocumentType { get; set; }

    public decimal QuantityBefore { get; set; }

    public decimal QuantityAfter { get; set; }
    //public decimal Quantity { get; set; }
    public Guid? ProductPackageId { get; set; }
    public decimal EnteredQuantity { get; set; }
    public decimal PackageMultiplier { get; set; }
    public decimal NormalizedQuantity { get; set; }
    public decimal ReservedBefore { get; set; }
    public decimal ReservedAfter { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalCost { get; set; }
    public Guid CurrencyId { get; set; }

    public string Notes { get; set; } = string.Empty;
    public MovementDirection MovementDirection { get; set; }
    //public MovementCategory MovementCategory { get; set; }

}
