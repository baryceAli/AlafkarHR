namespace Pricing.Pricings.Models;

public class PriceList : Aggregate<Guid>
{
    private readonly List<PriceListItem> _items = new();

    private PriceList()
    {
    }

    public string Name { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public Guid CompanyId { get; private set; }
    public string CurrencyCode { get; private set; } = "SAR";
    public bool IsDefault { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime EffectiveFrom { get; private set; }
    public DateTime? EffectiveTo { get; private set; }
    public IReadOnlyCollection<PriceListItem> Items => _items;

    public static PriceList Create(
        Guid id,
        string name,
        string code,
        Guid companyId,
        string currencyCode,
        bool isDefault,
        DateTime effectiveFrom,
        DateTime? effectiveTo,
        string createdBy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(currencyCode);

        if (effectiveTo.HasValue && effectiveTo.Value < effectiveFrom)
            throw new Exception("EffectiveTo cannot be earlier than EffectiveFrom.");

        return new PriceList
        {
            Id = id,
            Name = name,
            Code = code,
            CompanyId = companyId,
            CurrencyCode = currencyCode,
            IsDefault = isDefault,
            IsActive = true,
            EffectiveFrom = effectiveFrom,
            EffectiveTo = effectiveTo,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void Update(
        string name,
        string code,
        string currencyCode,
        bool isDefault,
        bool isActive,
        DateTime effectiveFrom,
        DateTime? effectiveTo,
        string modifiedBy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(currencyCode);

        if (effectiveTo.HasValue && effectiveTo.Value < effectiveFrom)
            throw new Exception("EffectiveTo cannot be earlier than EffectiveFrom.");

        Name = name;
        Code = code;
        CurrencyCode = currencyCode;
        IsDefault = isDefault;
        IsActive = isActive;
        EffectiveFrom = effectiveFrom;
        EffectiveTo = effectiveTo;
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
