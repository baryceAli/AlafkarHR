namespace SharedWithUI.SharedDtos;

public class SmartLinkSummaryResultDto
{
    public PartnerSmartLinkSummaryDto PartnerLinks { get; set; } = new();
    public ProductSmartLinkSummaryDto ProductLinks { get; set; } = new();
}
