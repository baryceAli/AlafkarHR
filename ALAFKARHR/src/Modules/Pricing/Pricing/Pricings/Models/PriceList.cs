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
        List<PriceListItem> priceListItems,
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

        var activeValues = _items.Where(v => !v.IsDeleted).ToList();
        var activeIds = activeValues.Select(v => v.Id).ToHashSet();

        // Add + Update
        foreach (var l in priceListItems)
        {
            if (l.Id == Guid.Empty)
            {
                AddPriceListItem(
                    l.ProductSkuId,
                    l.UnitId,
                    l.UnitPrice,
                    l.MinQuantity,
                    //l.EffectiveFrom,
                    //l.EffectiveTo,
                    modifiedBy);
                continue;
            }

            // 🚨 ONLY validate against ACTIVE values
            if (!activeIds.Contains(l.Id))
                throw new Exception($"Invalid or deleted Order items Id: {l.Id}");


            var existingValue = activeValues.First(ev => ev.Id == l.Id);
            existingValue.Update(l.ProductSkuId, l.UnitId, l.UnitPrice, l.MinQuantity, modifiedBy);
        }

        // Remove
        var dtoIds = priceListItems
            .Where(v => v.Id != Guid.Empty)
            .Select(v => v.Id)
            .ToHashSet();

        var valuesToRemove = activeValues
            .Where(ev => !dtoIds.Contains(ev.Id))
            .ToList();

        foreach (var value in valuesToRemove)
        {
            value.Remove(modifiedBy);
        }

    }

    public void Remove(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    public void AddPriceListItem(
        Guid productSkuId, 
        Guid? unitId, 
        decimal unitPrice, 
        decimal? minQuantity, 
        //DateTime effectiveFrom, 
        //DateTime?effectiveTo,
        string createdBy)

    {
        _items.Add(PriceListItem.Create(Guid.NewGuid(), Id, productSkuId, unitId, unitPrice, minQuantity, createdBy));
    }
    public void UpdatePriceListItem()
    {

    }
}
