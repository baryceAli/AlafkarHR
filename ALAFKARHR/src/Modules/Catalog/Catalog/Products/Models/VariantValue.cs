namespace Catalog.Products.Models;

public class VariantValue:Entity<Guid>
{
    public Guid VariantId { get; private set; }

    public string Value { get; private set; } = default!;
    public string ValueEng { get; private set; } = default!;

    private VariantValue() { }

    public static VariantValue Create(Guid id, Guid variantId, string value, string valueEng)
    {
        ArgumentException.ThrowIfNullOrEmpty(value);
        ArgumentException.ThrowIfNullOrEmpty(valueEng);

        return new VariantValue
        {
            Id = id,
            VariantId = variantId,
            Value = value,
            ValueEng = valueEng
        };
    }

    public void Update(string value, string valueEng)
    {
        ArgumentException.ThrowIfNullOrEmpty(value);
        ArgumentException.ThrowIfNullOrEmpty(valueEng);

        Value = value;
        ValueEng = valueEng;
    }
}
