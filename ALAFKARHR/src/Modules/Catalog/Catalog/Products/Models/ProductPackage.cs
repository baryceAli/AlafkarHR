namespace Catalog.Products.Models;

public class ProductPackage : Aggregate<Guid>
{
    public string Name { get; private set; } // 250ml, 1L, 500g
    public string NameEng { get; private set; } // 250ml, 1L, 500g
    public decimal Quantity { get; private set; }
    public Guid? UnitId { get; private set; }
    public string? Barcode { get; private set; }
    public Guid CompanyId { get; set; }
    private ProductPackage() { }

    internal ProductPackage(Guid id, 
                            string name, 
                            string nameEng, 
                            decimal quantity,
                            Guid? unitId,
                            string? barcode,
                            Guid companyId,
                            string createdBy)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentNullException.ThrowIfNullOrEmpty(nameEng);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);

        Id = id;
        //UnitId = unitId;
        Name = name;
        NameEng = nameEng;
        Quantity = quantity;
        UnitId = unitId;
        Barcode = NormalizeBarcode(barcode);
        CompanyId = companyId;
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
        decimal quantity,
        Guid? unitId,
        string? barcode,
        Guid companyId,
        string createdBy)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentNullException.ThrowIfNullOrEmpty(nameEng);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
        //UnitId = unitId;
        //ProductId = productId;
        return new ProductPackage
        {
            Id = id,
            Name = name,
            NameEng = nameEng,
            Quantity = quantity,
            UnitId = unitId,
            Barcode = NormalizeBarcode(barcode),
            CompanyId=companyId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
        
    }
    public void Update(string packageName, 
        string packageNameEng, 
        decimal quantity, 
        Guid? unitId,
        string? barcode,
        //decimal packagePrice, 
        string modifiedBy)
    {
        ArgumentException.ThrowIfNullOrEmpty(packageName);
        ArgumentNullException.ThrowIfNullOrEmpty(packageNameEng);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);

        //UnitId = unitId;
        //ProductId = productId;
        Name = packageName;
        NameEng = packageNameEng;
        Quantity = quantity;
        UnitId = unitId;
        Barcode = NormalizeBarcode(barcode);
        //PackagePrice = packagePrice;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }
    public void Remove(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    private static string? NormalizeBarcode(string? barcode)
        => string.IsNullOrWhiteSpace(barcode) ? null : barcode.Trim();
}
