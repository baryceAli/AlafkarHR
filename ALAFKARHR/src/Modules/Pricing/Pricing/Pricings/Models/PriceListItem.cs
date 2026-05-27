namespace Pricing.Pricings.Models;

public class PriceListItem : Entity<Guid>
{
    private PriceListItem()
    {
    }

    public Guid PriceListId { get; private set; }
    public Guid ProductSkuId { get; private set; }
    public Guid? UnitId { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal? MinQuantity { get; private set; }
    //public bool IsActive { get; private set; }
    //public DateTime EffectiveFrom { get; private set; }
    //public DateTime? EffectiveTo { get; private set; }

    public static PriceListItem Create(
        Guid id,
        Guid priceListId,
        Guid productSkuId,
        Guid? unitId,
        decimal unitPrice,
        decimal? minQuantity,
        //bool isActive,
        //DateTime effectiveFrom,
        //DateTime? effectiveTo,
        string createdBy)
    {
        if (unitPrice < 0)
            throw new Exception("Unit price cannot be negative.");

        if (minQuantity.HasValue && minQuantity.Value <= 0)
            throw new Exception("Minimum quantity must be greater than zero.");

        //if (effectiveTo.HasValue && effectiveTo.Value < effectiveFrom)
        //    throw new Exception("EffectiveTo cannot be earlier than EffectiveFrom.");

        return new PriceListItem
        {
            Id = id,
            PriceListId = priceListId,
            ProductSkuId = productSkuId,
            UnitId = unitId,
            UnitPrice = unitPrice,
            MinQuantity = minQuantity,
            //IsActive = true,
            //EffectiveFrom = effectiveFrom,
            //EffectiveTo = effectiveTo,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void Update(
        Guid productSkuId,
        Guid? unitId,
        decimal unitPrice,
        decimal? minQuantity,
        //bool isActive,
        //DateTime effectiveFrom,
        //DateTime? effectiveTo,
        string modifiedBy)
    {
        if (unitPrice < 0)
            throw new Exception("Unit price cannot be negative.");

        if (minQuantity.HasValue && minQuantity.Value <= 0)
            throw new Exception("Minimum quantity must be greater than zero.");

        //if (effectiveTo.HasValue && effectiveTo.Value < effectiveFrom)
        //    throw new Exception("EffectiveTo cannot be earlier than EffectiveFrom.");

        ProductSkuId = productSkuId;
        UnitId = unitId;
        UnitPrice = unitPrice;
        MinQuantity = minQuantity;
        //IsActive = isActive;
        //EffectiveFrom = effectiveFrom;
        //EffectiveTo = effectiveTo;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void Remove(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }
}
