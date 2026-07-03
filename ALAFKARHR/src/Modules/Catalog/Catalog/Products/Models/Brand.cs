namespace Catalog.Products.Models;

public class Brand : Aggregate<Guid>
{
    public string Name { get; private set; } = string.Empty;
    public string NameEng { get; private set; } = default!;
    public Guid CompanyId { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; } = true;
    private Brand() { }

    internal Brand (Guid id, string name, string nameEng,Guid companyId,string? createdBy, string? description = "")
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
    public static Brand Create(Guid id, string name,string nameEng,Guid companyId, string? createdBy, string? description = "")
    {
        return new Brand()
        {
            Id = id,
            Name = name,
            NameEng = nameEng,
            CompanyId=companyId,
            CreatedBy = createdBy,
            Description = description,
            IsActive = true,
            CreatedAt= DateTime.UtcNow

        };
    }
    [JsonConstructor]
    public Brand(Guid id, string name,string nameEng,Guid companyId, string? description = "")
    {
        Id = id;
        Name = name;
        NameEng = nameEng;
        CompanyId= companyId;
        Description = description;
        IsActive = true;
    }

    public void Update(string name,string nameEng,string modifiedBy, string? description = "")
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentException.ThrowIfNullOrEmpty(nameEng);
        Name = name;
        NameEng = nameEng;
        Description = description;
        ModifiedAt= DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void Remove(string deletedBy)
    {
        DeletedAt = DateTime.UtcNow;
        IsDeleted = true;
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
}
