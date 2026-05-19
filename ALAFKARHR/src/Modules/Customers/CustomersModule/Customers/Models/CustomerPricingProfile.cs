namespace CustomersModule.Customers.Models;

public class CustomerPricingProfile : Aggregate<Guid>
{
    public Guid CustomerId { get; private set; }

    public Guid PriceListId { get; private set; }

    public decimal? DiscountPercentage { get; private set; }

    public bool AllowAdditionalDiscounts { get; private set; }

    public DateTime EffectiveFrom { get; private set; }

    public DateTime? EffectiveTo { get; private set; }

    public Guid CompanyId { get; set; }
    public CustomerPricingProfile() { }

    public static CustomerPricingProfile Create(
        Guid id,
        Guid customerId,
        Guid priceListId,
        bool allowAdditionalDiscounts,
        DateTime effectiveFrom,
        DateTime? effectiveTo,
        Guid companyId,
        string createdBy)
    {
        return new CustomerPricingProfile
        {
            Id = id,
            CustomerId = customerId,
            PriceListId = priceListId,
            AllowAdditionalDiscounts = allowAdditionalDiscounts,
            EffectiveFrom = effectiveFrom,
            EffectiveTo = effectiveTo,
            CompanyId = companyId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }
    public void Update(
        //Guid customerId,
        Guid priceListId,
        bool allowAdditionalDiscounts,
        DateTime effectiveFrom,
        DateTime? effectiveTo,
        string modifiedBy)
    {
        //CustomerId = customerId;
        PriceListId= priceListId;
        AllowAdditionalDiscounts= allowAdditionalDiscounts;
        EffectiveFrom= effectiveFrom;
        EffectiveTo= effectiveTo;
        ModifiedAt= DateTime.UtcNow;
        ModifiedBy= modifiedBy;
    }

    public void Remove(string deletedBy)
    {
        IsDeleted = true;
        DeletedBy = deletedBy;
        DeletedAt = DateTime.UtcNow;
    }
}