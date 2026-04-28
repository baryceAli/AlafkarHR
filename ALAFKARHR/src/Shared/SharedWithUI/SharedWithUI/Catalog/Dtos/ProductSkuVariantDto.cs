namespace SharedWithUI.Catalog.Dtos;

public class ProductSkuVariantDto
{
    public Guid Id { get; set; }
    public Guid ProductSkuId { get; private set; }
    public Guid VariantId { get; private set; }
    public Guid VariantValueId { get; private set; }


}
