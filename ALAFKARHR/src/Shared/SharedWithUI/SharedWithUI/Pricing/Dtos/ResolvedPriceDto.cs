namespace SharedWithUI.Pricing.Dtos;

public class ResolvedPriceDto
{
    public Guid ProductSkuId { get; set; }
    public Guid? PriceListId { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountRate { get; set; }
    public decimal TaxRate { get; set; }
    public string PriceSource { get; set; } = string.Empty;
    public Guid? SourceId { get; set; }
    public decimal SourceUnitPrice { get; set; }
    public decimal? PromotionUnitPrice { get; set; }
    public decimal BulkDiscountRate { get; set; }
    public decimal BulkDiscountAmount { get; set; }
    public decimal CustomerDiscountRate { get; set; }
    public decimal CustomerDiscountAmount { get; set; }
    public string? CouponCode { get; set; }
    public string? CouponStatus { get; set; }
    public string? CouponDiscountType { get; set; }
    public decimal? CouponDiscountValue { get; set; }
    public decimal CouponDiscountAmount { get; set; }
    public decimal TaxableAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal FinalUnitAmount { get; set; }
    public decimal LineSubtotal { get; set; }
    public decimal LineTotal { get; set; }
}
