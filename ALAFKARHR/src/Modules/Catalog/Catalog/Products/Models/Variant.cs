namespace Catalog.Products.Models;

public class Variant : Aggregate<Guid>
{
    public string Name { get; private set; } = default!;
    public string NameEng { get; private set; } = default!;
    public Guid CompanyId { get; private set; }

    private readonly List<VariantValue> _values = new();
    public IReadOnlyCollection<VariantValue> Values => _values;
    private Variant() { }

    internal Variant(Guid id, string name, string nameEng,Guid companyId, string createdBy)
    {
        Id = id;
        Name = name;
        NameEng = nameEng;
        CompanyId = companyId;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    [JsonConstructor]
    public Variant(Guid id, string name, string nameEng,Guid companyId)
    {
        Id = id;
        Name = name;
        NameEng = nameEng;
        CompanyId= companyId;
    }
    public static Variant Create(Guid id, string name, string nameEng,Guid companyId, string createdBy)
    {
        return new Variant()
        {
            Id = id,
            Name = name,
            NameEng = nameEng,
            CompanyId=companyId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy,

        };
    }
    public void Update(string name, string nameEng, string modifiedBy)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentException.ThrowIfNullOrEmpty(nameEng);
        Name = name;
        NameEng = nameEng;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }
    public void Remove(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt= DateTime.UtcNow;
        DeletedBy = deletedBy;
    }
}
