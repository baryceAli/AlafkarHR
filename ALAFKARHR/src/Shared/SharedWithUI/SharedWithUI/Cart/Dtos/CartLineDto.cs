namespace SharedWithUI.Cart.Dtos;

public class CartLineDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid ProductSkuId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductNameEng { get; set; } = string.Empty;
    public string SkuCode { get; set; } = string.Empty;
    public Guid UnitOfMeasureId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountRate { get; set; }
    public decimal TaxRate { get; set; }
    public string? Notes { get; set; }
    public decimal NetAmount => (Quantity * UnitPrice) - ((Quantity * UnitPrice) * DiscountRate / 100m);
    public decimal TaxAmount => NetAmount * TaxRate / 100m;
    public decimal TotalAmount => NetAmount + TaxAmount;
}
