namespace Catalog.Products.Models;

public class Unit : Aggregate<Guid>
{
    public string UnitName { get; private set; } = string.Empty;
    public string UnitNameEng { get; private set; } = default!;
    public Guid CompanyId { get; set; }
    private Unit() { }

    internal Unit(Guid id,string unitName, string unitNameEng, string createdBy, Guid companyId)
    {
        Id = id;
        UnitName = unitName;
        UnitNameEng = unitNameEng;
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
    public static Unit Create(Guid id,string unitName, string unitNameEng,Guid companyId, string createdBy)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(unitName);
        ArgumentNullException.ThrowIfNullOrEmpty(unitNameEng);
        return new Unit()
        {
            Id = id,
            UnitName = unitName,
            UnitNameEng = unitNameEng,
            CompanyId= companyId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }
    public void Update(string unitName, string unitNameEng, string modifiedBy)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(unitName);
        ArgumentNullException.ThrowIfNullOrEmpty(UnitNameEng);

        UnitName = unitName;
        UnitNameEng = unitNameEng;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
        
    }
    public void Remove(string deletedBy)
    {
        DeletedAt= DateTime.UtcNow;
        DeletedBy = deletedBy;
    }
}
