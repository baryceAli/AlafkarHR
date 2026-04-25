namespace Catalog.Products.Models;

public class Variant : Aggregate<Guid>
{
    public string Name { get; private set; } = default!;
    public string NameEng { get; private set; } = default!;
    public Guid CompanyId { get; private set; }
    public string? Description { get; private set; } = default!;

    private Variant() { }

    internal Variant(Guid id, string name, string nameEng,Guid companyId, string createdBy, string? description = "")
    {
        Id = id;
        Name = name;
        NameEng = nameEng;
        Description = description;
        CompanyId = companyId;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    [JsonConstructor]
    public Variant(Guid id, string name, string nameEng,Guid companyId, string? description = "")
    {
        Id = id;
        Name = name;
        NameEng = nameEng;
        CompanyId= companyId;
        Description = description;
    }
    public static Variant Create(Guid id, string name, string nameEng,Guid companyId, string createdBy, string? description = "")
    {
        return new Variant()
        {
            Id = id,
            Name = name,
            NameEng = nameEng,
            CompanyId=companyId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy,
            Description = description

        };
    }
    public void Update(string name, string nameEng, string modifiedBy, string? description = "")
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentException.ThrowIfNullOrEmpty(nameEng);
        Name = name;
        NameEng = nameEng;
        Description = description;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }
    public void Remove(string deletedBy)
    {
        DeletedAt= DateTime.UtcNow;
        DeletedBy = deletedBy;
    }
}
