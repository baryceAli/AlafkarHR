namespace Catalog.Products.Models;

public class Category : Aggregate<Guid>
{
    public string Name { get; private set; } = string.Empty;
    public string NameEng { get; set; } = default!; 
    public string? Description { get; private set; } = string.Empty;

    private Category() { }

    internal Category(Guid id,string name, string nameEng, string createdBy, string? description = "")
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
    [JsonConstructor]
    public Category(Guid id,string name, string nameEng, string? description = "")
    {
        Id = id;
        Name = name;
        NameEng = nameEng;
        Description = description;
    }
    public static Category Create(Guid id, string name, string nameEng, string?description, string? createdBy)
    {
        return new Category()
        {
            Id = id,
            Name = name,
            NameEng = nameEng,
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
