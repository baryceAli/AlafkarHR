namespace SharedWithUI.Pricing.Dtos;

public class ResolvedPriceDto
{
    public Guid ProductSkuId { get; set; }
    public Guid? PriceListId { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountRate { get; set; }
    public decimal TaxRate { get; set; }
    public string PriceSource { get; set; } = string.Empty;
}
