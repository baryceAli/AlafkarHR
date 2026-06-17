namespace Pricing.Pricings.Models;

public class CustomerSalesContract : Aggregate<Guid>
{
    private readonly List<CustomerSalesContractItem> _items = new();

    private CustomerSalesContract()
    {
    }

    public string Name { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public Guid CustomerId { get; private set; }
    public Guid CompanyId { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime EffectiveFrom { get; private set; }
    public DateTime? EffectiveTo { get; private set; }
    public IReadOnlyCollection<CustomerSalesContractItem> Items => _items;

    public static CustomerSalesContract Create(
        Guid id,
        string name,
        string code,
        Guid customerId,
        Guid companyId,
        DateTime effectiveFrom,
        DateTime? effectiveTo,
        string createdBy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        if (customerId == Guid.Empty) throw new ArgumentException("Customer is required.", nameof(customerId));
        if (companyId == Guid.Empty) throw new ArgumentException("Company is required.", nameof(companyId));
        if (effectiveTo.HasValue && effectiveTo.Value < effectiveFrom)
            throw new Exception("EffectiveTo cannot be earlier than EffectiveFrom.");

        return new CustomerSalesContract
        {
            Id = id,
            Name = name,
            Code = code,
            CustomerId = customerId,
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
        _items.Add(CustomerSalesContractItem.Create(Guid.NewGuid(), Id, productSkuId, unitId, unitPrice, minQuantity, createdBy));
    }

    public void Remove(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }
}
