namespace Catalog.Products.Models;

public class Brand : Aggregate<Guid>
{
    public string Name { get; private set; } = string.Empty;
    public string NameEng { get; set; } = default!;
    public string? Description { get; private set; }
    private Brand() { }

    internal Brand (Guid id, string name, string nameEng,string? createdBy, string? description = "")
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentException.ThrowIfNullOrEmpty(nameEng);
        Id = id;
        Name = name;
        NameEng = nameEng;
        Description = description;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;

        
    }
    public static Brand Create(Guid id, string name,string nameEng, string? createdBy, string? description = "")
    {
        return new Brand()
        {
            Id = id,
            Name = name,
            NameEng = nameEng,
            CreatedBy = createdBy,
            Description = description,
            CreatedAt= DateTime.UtcNow

        };
    }
    [JsonConstructor]
    public Brand(Guid id, string name,string nameEng, string? description = "")
    {
        Id = id;
        Name = name;
        NameEng = nameEng;
        Description = description;
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
        DeletedBy = deletedBy;
    }
}
