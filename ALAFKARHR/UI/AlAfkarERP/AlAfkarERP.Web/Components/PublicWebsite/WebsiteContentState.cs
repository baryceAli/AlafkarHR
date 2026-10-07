using Microsoft.AspNetCore.Components;
using SharedWithUI.GeneralSettings.Dtos;

namespace AlAfkarERP.Web.Components.PublicWebsite;

public sealed class WebsitePublishedCache
{
    private WebsiteSnapshot? latest;
    public void Remember(WebsiteSnapshot snapshot) => Volatile.Write(ref latest, snapshot.Clone());
    public WebsiteSnapshot? LastSuccessful() => Volatile.Read(ref latest)?.Clone();
}

public sealed class WebsiteContentState(NavigationManager navigation)
{
    public WebsiteSnapshot Snapshot { get; private set; } = WebsiteContentCatalog.Manifest.CreateSnapshot();
    public event Action? Changed;
    public bool English => new Uri(navigation.Uri).AbsolutePath.Equals("/en", StringComparison.OrdinalIgnoreCase) || new Uri(navigation.Uri).AbsolutePath.StartsWith("/en/", StringComparison.OrdinalIgnoreCase);
    public string Language => English ? "en" : "ar";
    public string Direction => English ? "ltr" : "rtl";
    public string? PreviewSession { get; set; }
    public bool IsEditing { get; set; } = true;
    public int PackageCategory { get; private set; }
    public void SetPackageCategory(int category) { PackageCategory = category; Changed?.Invoke(); }
    public void Set(WebsiteSnapshot snapshot) { Snapshot = snapshot; Changed?.Invoke(); }
    public WebsiteValue Value(string key, WebsiteCollectionItem? item = null) => item != null && item.Fields.TryGetValue(key, out var v) ? v : Snapshot.Fields.GetValueOrDefault(key) ?? new();
    public string Text(string key, WebsiteCollectionItem? item = null) { var v = Value(key, item); return English ? v.En : v.Ar; }
    public string Media(string key, WebsiteCollectionItem? item = null)
    {
        var v = Value(key, item);
        if (v.MediaId is not Guid id) return English ? v.En : v.Ar;
        return PreviewSession != null ? $"/publicwebsite-preview-media/{PreviewSession}/{id}" : $"/website/media/{id}";
    }
    public string Link(string key, WebsiteCollectionItem? item = null) => Localize(Text(key, item));
    public string Resource(string resource, string fallback, WebsiteCollectionItem? item = null) => Value(resource).MediaId.HasValue ? Media(resource) : Link(fallback, item);
    public string LocalizeEnglish(string path) => path == "/" ? "/en" : "/en" + path;
    public string Localize(string path)
    {
        var target = path.Split('#', '?')[0];
        if (English && WebsiteRoutes.Paths.Contains(target, StringComparer.OrdinalIgnoreCase)) return (target == "/" ? "/en" : "/en" + target) + path[target.Length..];
        return path;
    }
    public List<WebsiteCollectionItem> Items(string collection) => Snapshot.Collections.GetValueOrDefault(collection) ?? [];
    public string ElementId(string id, WebsiteCollectionItem item) => item.Original ? id : id + "-" + item.Id.ToString("N");
}

public abstract class WebsiteContentComponent : ComponentBase, IDisposable
{
    [Inject] public WebsiteContentState Content { get; set; } = default!;
    protected override void OnInitialized() => Content.Changed += Update;
    private void Update() => _ = InvokeAsync(StateHasChanged);
    public virtual void Dispose() => Content.Changed -= Update;
}
