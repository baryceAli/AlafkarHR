namespace Catalog.Products.Models;

public class Variant : Aggregate<Guid>
{
    public string Name { get; private set; } = string.Empty;
    public string NameEng { get; set; } = default!;

    public string? Description { get; private set; } = string.Empty;

    private Variant() { }

    internal Variant(Guid id, string name, string nameEng, string createdBy, string? description = "")
    {
        Id = id;
        Name = name;
        NameEng = nameEng;
        Description = description;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    [JsonConstructor]
    public Variant(Guid id, string name, string nameEng, string? description = "")
    {
        Id = id;
        Name = name;
        NameEng = nameEng;
        Description = description;
    }
    public static Variant Create(Guid id, string name, string nameEng, string createdBy, string? description = "")
    {
        return new Variant()
        {
            Id = id,
            Name = name,
            NameEng = nameEng,
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
