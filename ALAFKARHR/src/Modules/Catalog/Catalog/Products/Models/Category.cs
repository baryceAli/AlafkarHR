namespace Catalog.Products.Models;

public class Category : Aggregate<Guid>
{
    public string Name { get; private set; } = string.Empty;
    public string NameEng { get; private set; } = default!;
    public Guid CompanyId { get; private set; }
    public string? Description { get; private set; } = string.Empty;

    private Category() { }

    internal Category(Guid id,string name, string nameEng, Guid companyId, string createdBy, string? description = "")
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentException.ThrowIfNullOrEmpty(nameEng);
        Id = id;
        Name = name;
        NameEng = nameEng;
        CompanyId = companyId;
        Description = description;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }
    [JsonConstructor]
    public Category(Guid id,string name, string nameEng, Guid companyId, string? description = "")
    {
        Id = id;
        Name = name;
        NameEng = nameEng;
        CompanyId= companyId;
        Description = description;
    }
    public static Category Create(Guid id, string name, string nameEng,Guid companyId, string?description, string? createdBy)
    {
        return new Category()
        {
            Id = id,
            Name = name,
            NameEng = nameEng,
            CompanyId=companyId,
            Description = description,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
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
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }
}
