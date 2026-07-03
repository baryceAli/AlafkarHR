namespace Catalog.Products.Models;

public class ProductPackage : Aggregate<Guid>
{
    public string Name { get; private set; } // 250ml, 1L, 500g
    public string NameEng { get; private set; } // 250ml, 1L, 500g
    public decimal Quantity { get; private set; }
    public Guid? UnitId { get; private set; }
    public string? Barcode { get; private set; }
    public decimal? Weight { get; private set; }
    public decimal? Length { get; private set; }
    public decimal? Width { get; private set; }
    public decimal? Height { get; private set; }
    public string? Notes { get; private set; }
    public bool IsActive { get; private set; } = true;
    public Guid CompanyId { get; set; }
    private ProductPackage() { }

    internal ProductPackage(Guid id, 
                            string name, 
                            string nameEng, 
                            decimal quantity,
                            Guid? unitId,
                            string? barcode,
                            decimal? weight,
                            decimal? length,
                            decimal? width,
                            decimal? height,
                            string? notes,
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
        SetPackageTypeMetadata(weight, length, width, height, notes);
        IsActive = true;
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
        decimal? weight,
        decimal? length,
        decimal? width,
        decimal? height,
        string? notes,
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
            Weight = NormalizeOptionalMeasurement(weight, nameof(weight)),
            Length = NormalizeOptionalMeasurement(length, nameof(length)),
            Width = NormalizeOptionalMeasurement(width, nameof(width)),
            Height = NormalizeOptionalMeasurement(height, nameof(height)),
            Notes = NormalizeNotes(notes),
            IsActive = true,
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
        decimal? weight,
        decimal? length,
        decimal? width,
        decimal? height,
        string? notes,
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
        SetPackageTypeMetadata(weight, length, width, height, notes);
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

    public void Archive(string modifiedBy)
    {
        IsActive = false;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void Activate(string modifiedBy)
    {
        IsActive = true;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    private static string? NormalizeBarcode(string? barcode)
        => string.IsNullOrWhiteSpace(barcode) ? null : barcode.Trim();

    private void SetPackageTypeMetadata(decimal? weight, decimal? length, decimal? width, decimal? height, string? notes)
    {
        Weight = NormalizeOptionalMeasurement(weight, nameof(weight));
        Length = NormalizeOptionalMeasurement(length, nameof(length));
        Width = NormalizeOptionalMeasurement(width, nameof(width));
        Height = NormalizeOptionalMeasurement(height, nameof(height));
        Notes = NormalizeNotes(notes);
    }

    private static decimal? NormalizeOptionalMeasurement(decimal? value, string name)
    {
        if (!value.HasValue)
            return null;

        if (value.Value < 0)
            throw new ArgumentOutOfRangeException(name, "Package measurements cannot be negative.");

        return value.Value;
    }

    private static string? NormalizeNotes(string? notes)
        => string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
}
