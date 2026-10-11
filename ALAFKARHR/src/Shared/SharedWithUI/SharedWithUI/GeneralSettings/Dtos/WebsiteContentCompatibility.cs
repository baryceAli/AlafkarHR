namespace SharedWithUI.GeneralSettings.Dtos;

/// <summary>Adapts catalog additions without rewriting stored drafts or historical revisions.</summary>
public static class WebsiteContentCompatibility
{
    public static WebsiteSnapshot UnpublishedDefaults()
    {
        var snapshot = WebsiteContentCatalog.Manifest.CreateSnapshot();
        foreach (var path in WebsiteServicePages.Sections.Keys)
            foreach (var section in snapshot.Pages[path]) section.Visible = false;
        return snapshot;
    }

    public static WebsiteSnapshot Adapt(WebsiteSnapshot source, WebsiteManifest? catalog = null)
    {
        var manifest = catalog ?? WebsiteContentCatalog.Manifest;
        if (source.SchemaVersion != 1) throw new InvalidOperationException("Unsupported website content schema.");
        var result = source.Clone();
        var defaults = manifest.CreateSnapshot();
        foreach (var (key, value) in defaults.Fields) result.Fields.TryAdd(key, value);
        foreach (var (page, sections) in defaults.Pages)
        {
            if (!result.Pages.TryGetValue(page, out var existing)) result.Pages[page] = existing = [];
            foreach (var section in sections)
                if (existing.All(x => x.Key != section.Key)) existing.Add(new() { Key = section.Key, Visible = false });
        }
        // New collections stay empty in old snapshots; applying approved content fills them.
        foreach (var collection in defaults.Collections.Keys) result.Collections.TryAdd(collection, []);
        return result;
    }

    public static WebsiteSnapshot ApprovedReplacement(WebsiteSnapshot source)
    {
        var manifest = WebsiteContentCatalog.Manifest;
        var replacement = manifest.CreateSnapshot();
        foreach (var field in manifest.Fields.Where(x => x.Kind is "image" or "pdf" or "video" or "audio" or "download"))
        {
            var id = source.Fields.GetValueOrDefault(field.Key)?.MediaId;
            if (id.HasValue) replacement.Fields[field.Key].MediaId = id;
        }
        foreach (var (target, original) in manifest.MediaAliases)
        {
            var id = source.Fields.GetValueOrDefault(target)?.MediaId ?? source.Fields.GetValueOrDefault(original)?.MediaId;
            // Original collection cards can carry an uploaded image override.
            id ??= source.Collections.Values.SelectMany(x => x).Where(x => x.Original)
                .Select(x => x.Fields.GetValueOrDefault(original)?.MediaId).FirstOrDefault(x => x.HasValue);
            if (id.HasValue) replacement.Fields[target].MediaId = id;
        }
        return replacement;
    }
}

public static class WebsiteServicePages
{
    public static readonly IReadOnlyDictionary<string, string> Sections = new Dictionary<string, string>
    {
        ["/services/iftar-haram"] = "ApprovedIftar",
        ["/services/suqya"] = "ApprovedSuqya",
        ["/services/meals"] = "ApprovedMeals",
        ["/services/food-baskets"] = "ApprovedBaskets",
        ["/services/hajj-season"] = "ApprovedHajj",
        ["/services/pilgrim-housing"] = "ApprovedHousing"
    };
    public static bool IsVisible(WebsiteSnapshot snapshot, string path) =>
        snapshot.Pages.TryGetValue(path, out var sections) &&
        (Sections.TryGetValue(path, out var primary) ? sections.Any(x => x.Key == primary && x.Visible) : sections.Any(x => x.Visible));
}

public static class WebsiteWhatsApp
{
    public const string Number = "966580007040";
    public const string Phone = "+966580007040";
    public const string Email = "info@alafkarsa.com";
    public static readonly string[] ServiceKeys = ["iftar", "suqya", "meals", "baskets", "hajj", "housing", "infrastructure", "other"];
    public static string? ServiceKey(string value)
    {
        var key = value switch
        {
            "ApprovedIftar" or "iftar-haram" or "إفطار صائم" => "iftar",
            "ApprovedSuqya" or "سقيا" => "suqya",
            "ApprovedMeals" => "meals",
            "ApprovedBaskets" or "food-baskets" or "سلال غذائية" => "baskets",
            "ApprovedHajj" or "hajj-season" or "إعاشة حجاج" => "hajj",
            "ApprovedHousing" or "pilgrim-housing" => "housing",
            "نقل وتخزين مبرد" => "infrastructure",
            _ => value
        };
        return ServiceKeys.Contains(key) ? key : null;
    }
    public static string Link(string message) => $"https://wa.me/{Number}?text={Uri.EscapeDataString(message)}";
    public static string Message(IEnumerable<KeyValuePair<string, string>> fields) =>
        string.Join("\n", fields.Where(x => !string.IsNullOrWhiteSpace(x.Value)).Select(x => $"{x.Key}: {x.Value.Trim()}"));
}
