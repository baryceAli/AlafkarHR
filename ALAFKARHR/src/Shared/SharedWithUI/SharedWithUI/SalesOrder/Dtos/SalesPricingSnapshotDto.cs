namespace SharedWithUI.SalesOrder.Dtos;

public class SalesPricingSnapshotDto
{
    public string? PriceSource { get; set; }
    public Guid? PriceSourceId { get; set; }
    public decimal? SourceUnitPrice { get; set; }
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
    public decimal FinalUnitAmount { get; set; }
    public bool IsManualPriceOverride { get; set; }
    public string? PriceOverrideBy { get; set; }
    public DateTime? PriceOverrideAt { get; set; }
}
