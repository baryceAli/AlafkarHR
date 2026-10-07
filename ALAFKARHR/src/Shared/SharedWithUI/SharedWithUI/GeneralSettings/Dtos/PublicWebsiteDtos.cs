using System.Text.Json;

namespace SharedWithUI.GeneralSettings.Dtos;

public sealed class WebsiteSnapshot
{
    public int SchemaVersion { get; set; } = 1;
    public Dictionary<string, WebsiteValue> Fields { get; set; } = [];
    public Dictionary<string, List<WebsiteSection>> Pages { get; set; } = [];
    public Dictionary<string, List<WebsiteCollectionItem>> Collections { get; set; } = [];
    public WebsiteSnapshot Clone() => JsonSerializer.Deserialize<WebsiteSnapshot>(JsonSerializer.Serialize(this))!;
}

public sealed class WebsiteValue
{
    public string Ar { get; set; } = "";
    public string En { get; set; } = "";
    public Guid? MediaId { get; set; }
}

public sealed class WebsiteSection
{
    public string Key { get; set; } = "";
    public bool Visible { get; set; } = true;
}

public sealed class WebsiteCollectionItem
{
    public Guid Id { get; set; }
    public bool Original { get; set; }
    public string TemplateKey { get; set; } = "";
    public Dictionary<string, WebsiteValue> Fields { get; set; } = [];
}

public sealed class WebsiteFieldDefinition
{
    public string Key { get; set; } = "";
    public string Section { get; set; } = "";
    public string Kind { get; set; } = "text";
    public string Ar { get; set; } = "";
    public string En { get; set; } = "";
    public bool Required { get; set; } = true;
}

public sealed class WebsiteManifest
{
    public List<WebsiteFieldDefinition> Fields { get; set; } = [];
    public Dictionary<string, List<string>> Pages { get; set; } = [];
    public Dictionary<string, List<string>> Collections { get; set; } = [];
    public Dictionary<string, List<string>> SectionChildren { get; set; } = [];
    public WebsiteSnapshot CreateSnapshot() => new()
    {
        Fields = Fields.ToDictionary(x => x.Key, x => new WebsiteValue { Ar = x.Ar, En = x.En }),
        Pages = Pages.ToDictionary(x => x.Key, x => x.Value.Select(k => new WebsiteSection { Key = k }).ToList()),
        Collections = Collections.ToDictionary(x => x.Key, x => x.Value.Select(k => new WebsiteCollectionItem { Id = Guid.NewGuid(), Original = true, TemplateKey = k }).ToList())
    };
}

public sealed record WebsiteDraftDto(Guid Token, WebsiteSnapshot Content, DateTime UpdatedAt, string UpdatedBy);
public sealed record WebsiteSaveRequest(Guid ExpectedToken, WebsiteSnapshot Content);
public sealed record WebsiteVersionRequest(Guid ExpectedToken);
public sealed record WebsiteRevisionDto(Guid Id, DateTime PublishedAt, string PublishedBy);
public sealed record WebsiteMediaDto(Guid Id, string Name, string ContentType, long Size);

public static class WebsiteContentCatalog
{
    private static readonly Lazy<WebsiteManifest> Catalog = new(() =>
    {
        using var stream = typeof(WebsiteContentCatalog).Assembly.GetManifestResourceStream("SharedWithUI.PublicWebsite.manifest.json")!;
        return JsonSerializer.Deserialize<WebsiteManifest>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    });
    public static WebsiteManifest Manifest => Catalog.Value;
}
