namespace Pricing.Pricings.Models;

public class PromotionPrice : Aggregate<Guid>
{
    private readonly List<PromotionPriceItem> _items = new();

    private PromotionPrice()
    {
    }

    public string Name { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public Guid CompanyId { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime EffectiveFrom { get; private set; }
    public DateTime? EffectiveTo { get; private set; }
    public IReadOnlyCollection<PromotionPriceItem> Items => _items;

    public static PromotionPrice Create(
        Guid id,
        string name,
        string code,
        Guid companyId,
        DateTime effectiveFrom,
        DateTime? effectiveTo,
        string createdBy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        if (companyId == Guid.Empty) throw new ArgumentException("Company is required.", nameof(companyId));
        if (effectiveTo.HasValue && effectiveTo.Value < effectiveFrom)
            throw new Exception("EffectiveTo cannot be earlier than EffectiveFrom.");

        return new PromotionPrice
        {
            Id = id,
            Name = name,
            Code = code,
            CompanyId = companyId,
            IsActive = true,
            EffectiveFrom = effectiveFrom,
            EffectiveTo = effectiveTo,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void AddItem(Guid productSkuId, Guid? unitId, decimal unitPrice, decimal? minQuantity, string createdBy)
    {
        _items.Add(PromotionPriceItem.Create(Guid.NewGuid(), Id, productSkuId, unitId, unitPrice, minQuantity, createdBy));
    }

    public void Remove(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }
}
