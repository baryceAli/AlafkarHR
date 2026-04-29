namespace Catalog.Products.Models;

public class VariantValue:Entity<Guid>
{
    public Guid VariantId { get; private set; }

    public string Value { get; private set; } = default!;
    public string ValueEng { get; private set; } = default!;

    private VariantValue() { }

    public static VariantValue Create(Guid id, Guid variantId, string value, string valueEng, string createdBy)
    {
        ArgumentException.ThrowIfNullOrEmpty(value);
        ArgumentException.ThrowIfNullOrEmpty(valueEng);
        ArgumentException.ThrowIfNullOrEmpty(createdBy);

        return new VariantValue
        {
            Id = id,
            VariantId = variantId,
            Value = value,
            ValueEng = valueEng,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void Update(string value, string valueEng, string modifiedBy)
    {
        ArgumentException.ThrowIfNullOrEmpty(value);
        ArgumentException.ThrowIfNullOrEmpty(valueEng);

        Value = value;
        ValueEng = valueEng;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }
    public void Remove(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }
}
