namespace Pricing.Pricings.Models;

public class PromotionPriceItem : Entity<Guid>
{
    private PromotionPriceItem()
    {
    }

    public Guid PromotionPriceId { get; private set; }
    public Guid ProductSkuId { get; private set; }
    public Guid? UnitId { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal? MinQuantity { get; private set; }

    public static PromotionPriceItem Create(
        Guid id,
        Guid promotionPriceId,
        Guid productSkuId,
        Guid? unitId,
        decimal unitPrice,
        decimal? minQuantity,
        string createdBy)
    {
        if (promotionPriceId == Guid.Empty) throw new ArgumentException("Promotion is required.", nameof(promotionPriceId));
        if (productSkuId == Guid.Empty) throw new ArgumentException("Product SKU is required.", nameof(productSkuId));
        if (unitPrice < 0) throw new Exception("Unit price cannot be negative.");
        if (minQuantity.HasValue && minQuantity.Value < 0) throw new Exception("Minimum quantity cannot be negative.");

        return new PromotionPriceItem
        {
            Id = id,
            PromotionPriceId = promotionPriceId,
            ProductSkuId = productSkuId,
            UnitId = unitId,
            UnitPrice = unitPrice,
            MinQuantity = minQuantity,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }
}
