using System.ComponentModel.DataAnnotations;

namespace SharedWithUI.GeneralSettings.Dtos;

public static class HomePageTemplateKeys
{
    public const string CurrentStorefront = "CurrentStorefront";
    public const string MinimalistLanding = "MinimalistLanding";
    public const string SoftSaasLanding = "SoftSaasLanding";
    public const string BoldEnergeticLanding = "BoldEnergeticLanding";
    public const string CorporateTrustLanding = "CorporateTrustLanding";
    public const string ModernDarkModeLanding = "ModernDarkModeLanding";

    public static readonly string[] All =
    [
        CurrentStorefront,
        MinimalistLanding,
        SoftSaasLanding,
        BoldEnergeticLanding,
        CorporateTrustLanding,
        ModernDarkModeLanding
    ];

    public static bool IsValid(string? key)
        => All.Contains(key ?? string.Empty, StringComparer.Ordinal);
}

public class HomePageTemplateSummaryDto
{
    public string Key { get; set; } = HomePageTemplateKeys.CurrentStorefront;
    public string NameEn { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string DescriptionEn { get; set; } = string.Empty;
    public string DescriptionAr { get; set; } = string.Empty;
}

public class HomePageContentItemDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string TemplateKey { get; set; } = HomePageTemplateKeys.CurrentStorefront;

    [Required]
    [MaxLength(80)]
    public string SectionKey { get; set; } = string.Empty;

    [Required]
    [MaxLength(80)]
    public string FieldKey { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string ContentType { get; set; } = "Text";

    [MaxLength(2000)]
    public string TextEn { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string TextAr { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string ImagePath { get; set; } = string.Empty;

    [MaxLength(300)]
    public string AltTextEn { get; set; } = string.Empty;

    [MaxLength(300)]
    public string AltTextAr { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool IsVisible { get; set; } = true;
}

public class HomePageTemplateDto
{
    public Guid CompanyId { get; set; }
    public string ActiveTemplateKey { get; set; } = HomePageTemplateKeys.CurrentStorefront;
    public List<HomePageTemplateSummaryDto> Templates { get; set; } = [];
    public List<HomePageContentItemDto> ContentItems { get; set; } = [];
}

public class UpdateHomePageActiveTemplateDto
{
    [Required]
    public string ActiveTemplateKey { get; set; } = HomePageTemplateKeys.CurrentStorefront;
}

