namespace SharedWithUI.Orders.Dtos;

public class OrderIntakeLineDto
{
    public Guid Id { get; set; }
    public int LineNumber { get; set; }
    public Guid ProductId { get; set; }
    public Guid ProductSkuId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductNameEng { get; set; } = string.Empty;
    public string SkuCode { get; set; } = string.Empty;
    public Guid UnitOfMeasureId { get; set; }
    public decimal Quantity { get; set; }
    public decimal RequestedUnitPrice { get; set; }
    public decimal RequestedDiscountRate { get; set; }
    public decimal RequestedTaxRate { get; set; }
    public string? Notes { get; set; }
    public decimal NetAmount => (Quantity * RequestedUnitPrice) - ((Quantity * RequestedUnitPrice) * RequestedDiscountRate / 100m);
    public decimal TaxAmount => NetAmount * RequestedTaxRate / 100m;
    public decimal TotalAmount => NetAmount + TaxAmount;
}
