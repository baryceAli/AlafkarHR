namespace Pricing.Pricings.Models;

public enum CouponDiscountType
{
    Percentage = 1,
    FixedAmount = 2
}

public class DiscountCoupon : Aggregate<Guid>
{
    private DiscountCoupon()
    {
    }

    public string Code { get; private set; } = string.Empty;
    public Guid CompanyId { get; private set; }
    public Guid? CustomerId { get; private set; }
    public Guid? CustomerGroupId { get; private set; }
    public Guid? ProductSkuId { get; private set; }
    public decimal? MinimumOrderAmount { get; private set; }
    public CouponDiscountType DiscountType { get; private set; }
    public decimal DiscountValue { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime EffectiveFrom { get; private set; }
    public DateTime? EffectiveTo { get; private set; }

    public static DiscountCoupon Create(
        Guid id,
        string code,
        Guid companyId,
        CouponDiscountType discountType,
        decimal discountValue,
        DateTime effectiveFrom,
        DateTime? effectiveTo,
        string createdBy,
        Guid? customerId = null,
        Guid? customerGroupId = null,
        Guid? productSkuId = null,
        decimal? minimumOrderAmount = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        if (companyId == Guid.Empty) throw new ArgumentException("Company is required.", nameof(companyId));
        if (discountValue <= 0) throw new Exception("Discount value must be greater than zero.");
        if (minimumOrderAmount.HasValue && minimumOrderAmount.Value < 0)
            throw new Exception("Minimum order amount cannot be negative.");
        if (effectiveTo.HasValue && effectiveTo.Value < effectiveFrom)
            throw new Exception("EffectiveTo cannot be earlier than EffectiveFrom.");

        return new DiscountCoupon
        {
            Id = id,
            Code = code.Trim().ToUpperInvariant(),
            CompanyId = companyId,
            CustomerId = customerId,
            CustomerGroupId = customerGroupId,
            ProductSkuId = productSkuId,
            MinimumOrderAmount = minimumOrderAmount,
            DiscountType = discountType,
            DiscountValue = discountValue,
            IsActive = true,
            EffectiveFrom = effectiveFrom,
            EffectiveTo = effectiveTo,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void Remove(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }
}
