namespace CustomersModule.Customers.Models;

//Wholesale
//VIP
//Retail
//Distributor
//Government
public class CustomerGroup : Aggregate<Guid>
{
    public string Name { get; private set; }
    public string NameEng { get; set; }

    public string? Description { get; private set; }

    public decimal? DefaultDiscountPercentage { get; private set; }

    public Guid? DefaultPriceListId { get; private set; }

    public Guid CompanyId { get; set; }
    private CustomerGroup(){}
    public static CustomerGroup Create(
        Guid id,
        string name, 
        string nameEng,
        string?description, 
        decimal? defaultDiscountPercentage, 
        Guid? defaultPriceListId, 
        Guid companyId,
        string createdBy)
    {
        return new CustomerGroup
        {
            Id = id,
            Name = name,
            NameEng=nameEng,
            Description = description,
            DefaultDiscountPercentage = defaultDiscountPercentage,
            DefaultPriceListId = defaultPriceListId,
            CompanyId = companyId,
            CreatedAt=DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }
    public void Update(string name,string nameEng, string? description, decimal? defaultDiscountPercentage, Guid? defaultPriceListId, string modifiedBy)
    {
        Name=name;
        NameEng=nameEng;
        Description=description;
        DefaultDiscountPercentage=defaultDiscountPercentage;
        DefaultPriceListId=defaultPriceListId;
        ModifiedAt=DateTime.UtcNow;
        ModifiedBy=modifiedBy;
    }
    public void Remove(string deletedBy)
    {
        //cusotmer group
        DeletedBy=deletedBy;
        IsDeleted = true;
        DeletedAt=DateTime.UtcNow;
    }
}