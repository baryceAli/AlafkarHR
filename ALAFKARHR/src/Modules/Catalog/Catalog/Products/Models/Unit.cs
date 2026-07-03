namespace Catalog.Products.Models;

public class Unit : Aggregate<Guid>
{
    public string UnitName { get; private set; } = string.Empty;
    public string UnitNameEng { get; private set; } = default!;
    public string UnitCategory { get; private set; } = "General";
    public decimal ConversionFactor { get; private set; } = 1;
    public bool IsReferenceUnit { get; private set; }
    public Guid CompanyId { get; set; }
    private Unit() { }

    internal Unit(Guid id,string unitName, string unitNameEng, string unitCategory, decimal conversionFactor, bool isReferenceUnit, string createdBy, Guid companyId)
    {
        Id = id;
        UnitName = unitName;
        UnitNameEng = unitNameEng;
        UnitCategory = NormalizeCategory(unitCategory);
        ConversionFactor = conversionFactor;
        IsReferenceUnit = isReferenceUnit;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        CompanyId = companyId;
    }
    [JsonConstructor]
    public Unit(Guid id,string unitName, string unitNameEng, Guid companyId)
    {
        Id = id;
        UnitName = unitName;
        UnitNameEng = unitNameEng;
        CompanyId=companyId;
    }
    public static Unit Create(Guid id,string unitName, string unitNameEng, string unitCategory, decimal conversionFactor, bool isReferenceUnit, Guid companyId, string createdBy)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(unitName);
        ArgumentNullException.ThrowIfNullOrEmpty(unitNameEng);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(conversionFactor);
        return new Unit()
        {
            Id = id,
            UnitName = unitName,
            UnitNameEng = unitNameEng,
            UnitCategory = NormalizeCategory(unitCategory),
            ConversionFactor = conversionFactor,
            IsReferenceUnit = isReferenceUnit,
            CompanyId= companyId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }
    public void Update(string unitName, string unitNameEng, string unitCategory, decimal conversionFactor, bool isReferenceUnit, string modifiedBy)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(unitName);
        ArgumentNullException.ThrowIfNullOrEmpty(unitNameEng);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(conversionFactor);

        UnitName = unitName;
        UnitNameEng = unitNameEng;
        UnitCategory = NormalizeCategory(unitCategory);
        ConversionFactor = conversionFactor;
        IsReferenceUnit = isReferenceUnit;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
        
    }
    public void Remove(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt= DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    private static string NormalizeCategory(string? unitCategory)
        => string.IsNullOrWhiteSpace(unitCategory) ? "General" : unitCategory.Trim();
}
