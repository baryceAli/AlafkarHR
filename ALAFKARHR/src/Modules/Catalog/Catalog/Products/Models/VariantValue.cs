namespace Catalog.Products.Models;

public class VariantValue:Entity<Guid>
{
    public Guid VariantId { get; private set; }

    public string Value { get; private set; } = default!;
    public string ValueEng { get; private set; } = default!;
}
