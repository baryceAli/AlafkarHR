using Shared.DDD;

namespace SalesOrder.Orders.Models;

public class SalesQuotationLine : Entity<Guid>
{
    private SalesQuotationLine() { }

    public int LineNumber { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid ProductSkuId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public string ProductNameEng { get; private set; } = string.Empty;
    public string SkuCode { get; private set; } = string.Empty;
    public Guid UnitOfMeasureId { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal DiscountRate { get; private set; }
    public decimal TaxRate { get; private set; }
    public string? Notes { get; private set; }
    public string? PriceSource { get; private set; }
    public Guid? PriceSourceId { get; private set; }
    public decimal? SourceUnitPrice { get; private set; }
    public decimal? PromotionUnitPrice { get; private set; }
    public decimal BulkDiscountRate { get; private set; }
    public decimal BulkDiscountAmount { get; private set; }
    public decimal CustomerDiscountRate { get; private set; }
    public decimal CustomerDiscountAmount { get; private set; }
    public string? CouponCode { get; private set; }
    public string? CouponStatus { get; private set; }
    public string? CouponDiscountType { get; private set; }
    public decimal? CouponDiscountValue { get; private set; }
    public decimal CouponDiscountAmount { get; private set; }
    public decimal TaxableAmount { get; private set; }
    public decimal FinalUnitAmount { get; private set; }
    public decimal DiscountAmount => Quantity * UnitPrice * DiscountRate / 100m;
    public decimal NetAmount => Quantity * UnitPrice - DiscountAmount;
    public decimal TaxAmount => NetAmount * TaxRate / 100m;
    public decimal TotalAmount => NetAmount + TaxAmount;

    public static SalesQuotationLine Create(int lineNumber, SalesQuotationLineDto dto, string userId)
    {
        if (dto.ProductId == Guid.Empty) throw new Exception("Product is required.");
        if (dto.ProductSkuId == Guid.Empty) throw new Exception("Product SKU is required.");
        if (dto.UnitOfMeasureId == Guid.Empty) throw new Exception("Unit of measure is required.");
        if (dto.Quantity <= 0) throw new Exception("Quantity must be greater than zero.");

        return new SalesQuotationLine
        {
            Id = Guid.NewGuid(),
            LineNumber = lineNumber,
            ProductId = dto.ProductId,
            ProductSkuId = dto.ProductSkuId,
            ProductName = dto.ProductName,
            ProductNameEng = dto.ProductNameEng,
            SkuCode = dto.SkuCode,
            UnitOfMeasureId = dto.UnitOfMeasureId,
            Quantity = dto.Quantity,
            UnitPrice = dto.UnitPrice,
            DiscountRate = dto.DiscountRate,
            TaxRate = dto.TaxRate,
            Notes = dto.Notes,
            PriceSource = dto.Pricing.PriceSource,
            PriceSourceId = dto.Pricing.PriceSourceId,
            SourceUnitPrice = dto.Pricing.SourceUnitPrice,
            PromotionUnitPrice = dto.Pricing.PromotionUnitPrice,
            BulkDiscountRate = dto.Pricing.BulkDiscountRate,
            BulkDiscountAmount = dto.Pricing.BulkDiscountAmount,
            CustomerDiscountRate = dto.Pricing.CustomerDiscountRate,
            CustomerDiscountAmount = dto.Pricing.CustomerDiscountAmount,
            CouponCode = dto.Pricing.CouponCode,
            CouponStatus = dto.Pricing.CouponStatus,
            CouponDiscountType = dto.Pricing.CouponDiscountType,
            CouponDiscountValue = dto.Pricing.CouponDiscountValue,
            CouponDiscountAmount = dto.Pricing.CouponDiscountAmount,
            TaxableAmount = dto.Pricing.TaxableAmount,
            FinalUnitAmount = dto.Pricing.FinalUnitAmount,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };
    }
}
