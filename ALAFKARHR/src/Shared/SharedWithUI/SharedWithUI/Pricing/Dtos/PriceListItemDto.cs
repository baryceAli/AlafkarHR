namespace SharedWithUI.Pricing.Dtos;

public class PriceListItemDto
{
    public Guid Id { get; set; }
    public Guid PriceListId { get; set; }
    public Guid ProductSkuId { get; set; }
    public Guid? UnitId { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal? MinQuantity { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime EffectiveFrom { get; set; } = DateTime.UtcNow;
    public DateTime? EffectiveTo { get; set; }
}
