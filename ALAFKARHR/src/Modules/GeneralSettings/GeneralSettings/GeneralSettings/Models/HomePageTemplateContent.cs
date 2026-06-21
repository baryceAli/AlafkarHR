using Shared.DDD;

namespace GeneralSettings.GeneralSettings.Models;

public abstract class HomePageTemplateContent : Aggregate<Guid>
{
    public Guid CompanyId { get; protected set; }
    public string SectionKey { get; protected set; } = string.Empty;
    public string FieldKey { get; protected set; } = string.Empty;
    public string ContentType { get; protected set; } = "Text";
    public string TextEn { get; protected set; } = string.Empty;
    public string TextAr { get; protected set; } = string.Empty;
    public string ImagePath { get; protected set; } = string.Empty;
    public string AltTextEn { get; protected set; } = string.Empty;
    public string AltTextAr { get; protected set; } = string.Empty;
    public int SortOrder { get; protected set; }
    public bool IsVisible { get; protected set; } = true;

    public void Update(
        string contentType,
        string textEn,
        string textAr,
        string imagePath,
        string altTextEn,
        string altTextAr,
        int sortOrder,
        bool isVisible,
        string modifiedBy)
    {
        ContentType = contentType;
        TextEn = textEn;
        TextAr = textAr;
        ImagePath = imagePath;
        AltTextEn = altTextEn;
        AltTextAr = altTextAr;
        SortOrder = sortOrder;
        IsVisible = isVisible;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    internal void Initialize(
        Guid id,
        Guid companyId,
        string sectionKey,
        string fieldKey,
        string contentType,
        string textEn,
        string textAr,
        string imagePath,
        string altTextEn,
        string altTextAr,
        int sortOrder,
        bool isVisible,
        string createdBy)
    {
        Id = id;
        CompanyId = companyId;
        SectionKey = sectionKey;
        FieldKey = fieldKey;
        ContentType = contentType;
        TextEn = textEn;
        TextAr = textAr;
        ImagePath = imagePath;
        AltTextEn = altTextEn;
        AltTextAr = altTextAr;
        SortOrder = sortOrder;
        IsVisible = isVisible;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }
}

public class CurrentStorefrontHomePageContent : HomePageTemplateContent
{
    private CurrentStorefrontHomePageContent()
    {
    }

    public static CurrentStorefrontHomePageContent Create(HomePageContentSeed seed, string createdBy)
    {
        var content = new CurrentStorefrontHomePageContent();
        content.InitializeFromSeed(seed, createdBy);
        return content;
    }
}

public class CorporateShowcaseHomePageContent : HomePageTemplateContent
{
    private CorporateShowcaseHomePageContent()
    {
    }

    public static CorporateShowcaseHomePageContent Create(HomePageContentSeed seed, string createdBy)
    {
        var content = new CorporateShowcaseHomePageContent();
        content.InitializeFromSeed(seed, createdBy);
        return content;
    }
}

public class ProductHighlightHomePageContent : HomePageTemplateContent
{
    private ProductHighlightHomePageContent()
    {
    }

    public static ProductHighlightHomePageContent Create(HomePageContentSeed seed, string createdBy)
    {
        var content = new ProductHighlightHomePageContent();
        content.InitializeFromSeed(seed, createdBy);
        return content;
    }
}

public class CampaignLandingHomePageContent : HomePageTemplateContent
{
    private CampaignLandingHomePageContent()
    {
    }

    public static CampaignLandingHomePageContent Create(HomePageContentSeed seed, string createdBy)
    {
        var content = new CampaignLandingHomePageContent();
        content.InitializeFromSeed(seed, createdBy);
        return content;
    }
}

public class MinimalCatalogHomePageContent : HomePageTemplateContent
{
    private MinimalCatalogHomePageContent()
    {
    }

    public static MinimalCatalogHomePageContent Create(HomePageContentSeed seed, string createdBy)
    {
        var content = new MinimalCatalogHomePageContent();
        content.InitializeFromSeed(seed, createdBy);
        return content;
    }
}

public record HomePageContentSeed(
    Guid CompanyId,
    string SectionKey,
    string FieldKey,
    string ContentType,
    string TextEn,
    string TextAr,
    string ImagePath,
    string AltTextEn,
    string AltTextAr,
    int SortOrder,
    bool IsVisible);

internal static class HomePageTemplateContentFactoryExtensions
{
    public static void InitializeFromSeed(this HomePageTemplateContent content, HomePageContentSeed seed, string createdBy)
    {
        content.Initialize(
            Guid.NewGuid(),
            seed.CompanyId,
            seed.SectionKey,
            seed.FieldKey,
            seed.ContentType,
            seed.TextEn,
            seed.TextAr,
            seed.ImagePath,
            seed.AltTextEn,
            seed.AltTextAr,
            seed.SortOrder,
            seed.IsVisible,
            createdBy);
    }
}
