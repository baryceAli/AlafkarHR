namespace SharedWithUI.Pricing.Dtos;

public class PriceListDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Code { get; set; } = string.Empty;
    public Guid CompanyId { get; set; }
    public string CurrencyCode { get; set; } = "SAR";
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime EffectiveFrom { get; set; } = DateTime.UtcNow;
    public DateTime? EffectiveTo { get; set; }
    public List<PriceListItemDto> Items { get; set; } = new();
}
