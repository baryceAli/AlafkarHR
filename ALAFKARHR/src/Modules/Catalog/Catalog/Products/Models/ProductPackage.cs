namespace Catalog.Products.Models;

public class ProductPackage : Aggregate<Guid>
{
    //public Guid UnitId { get; private set; }
    //public Guid ProductId { get; private set; }
    public string Name { get; private set; }
    public string NameEng { get;private set; } 
    public double UnitsCount { get; private set; }
    //public decimal PackagePrice { get; set; }

    //this price is calculated price* unitRate --> the price value will come from product or ProductVariant if available
    //public decimal PackagePrice { get; set; }

    private ProductPackage() { }

    internal ProductPackage(Guid id, 
                            //Guid productId, 
                            string name, 
                            string nameEng, 
                            double unitsCount, 
                            decimal packagePrice,
                            string createdBy)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentNullException.ThrowIfNullOrEmpty(nameEng);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(unitsCount);

        Id = id;
        //UnitId = unitId;
        Name = name;
        NameEng = nameEng;
        UnitsCount = unitsCount;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    //[JsonConstructor]
    //public ProductPackage(Guid id, 
    //    Guid productId, 
    //    string packageName, 
    //    string packageNameEng, 
    //    double unitsCount,
    //    decimal packagePrice)
    //{
    //    Id = id;
    //    //UnitId = unitId;
    //    ProductId = productId;
    //    Name = packageName;
    //    NameEng = packageNameEng;
    //    UnitsCount = unitsCount;
    //    PackagePrice = packagePrice;
    //}

    public static ProductPackage Create(Guid id,
        string name,
        string nameEng,
        double unitsCount,
        string createdBy)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentNullException.ThrowIfNullOrEmpty(nameEng);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(unitsCount);

        //UnitId = unitId;
        //ProductId = productId;
        return new ProductPackage
        {
            Id = id,
            Name = name,
            NameEng = nameEng,
            UnitsCount = unitsCount,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
        
    }
    public void Update(string packageName, 
        string packageNameEng, 
        double unitsCount, 
        //decimal packagePrice, 
        string modifiedBy)
    {
        ArgumentException.ThrowIfNullOrEmpty(packageName);
        ArgumentNullException.ThrowIfNullOrEmpty(packageNameEng);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(unitsCount);

        //UnitId = unitId;
        //ProductId = productId;
        Name = packageName;
        NameEng = packageNameEng;
        UnitsCount = unitsCount;
        //PackagePrice = packagePrice;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }
    public void Remove(string deletedBy)
    {
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }
}
