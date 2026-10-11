global using Microsoft.Extensions.Configuration;
using System.Text.Json;
using SharedWithUI.GeneralSettings.Dtos;
using AlAfkarERP.Web.Components.PublicWebsite;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System.Text.RegularExpressions;

var checks = 0;
void Check(bool condition, string description)
{
    if (!condition) throw new InvalidOperationException(description);
    checks++;
}
var manifest = WebsiteContentCatalog.Manifest;
var approved = manifest.CreateSnapshot();
foreach (var path in WebsiteServicePages.Sections.Keys)
    Check(!WebsiteServicePages.IsVisible(WebsiteContentCompatibility.UnpublishedDefaults(), path), "Cold-start/no-publication fallback cannot expose a service: " + path);
Check(manifest.ApprovedPages.Count == 16, "Ten existing pages and six service pages must be present.");
foreach (var (path, sections) in manifest.ApprovedPages)
{
    Check(approved.Pages[path].Where(x => x.Visible).Select(x => x.Key).SequenceEqual(sections), "Approved section order: " + path);
    Check(approved.Pages[path].Where(x => x.Visible).All(x => manifest.SectionLayouts.ContainsKey(x.Key)), "Legacy demonstrations must not appear: " + path);
}
foreach (var field in manifest.Fields.Where(x => x.Required))
    Check(!string.IsNullOrWhiteSpace(field.Ar) && !string.IsNullOrWhiteSpace(field.En), "Required bilingual field: " + field.Key);
foreach (var (path, section) in WebsiteServicePages.Sections)
{
    Check(WebsiteRoutes.IsPublic(path) && WebsiteRoutes.IsPublic("/en" + path), "Both language routes: " + path);
    Check(WebsiteServicePages.IsVisible(approved, path), "Approved service page is available: " + path);
    Check(manifest.Fields.Any(x => x.Key == section + ".f001" && x.Kind == "seo"), "Service SEO/editor mapping: " + path);
    Check(manifest.Fields.Any(x => x.Key == section + ".f002" && x.Kind == "seo"), "Service description: " + path);
}
Check(WebsiteRoutes.ArabicPath("/en/services/meals/") == "/services/meals", "Language route normalisation.");
Check(!WebsiteRoutes.IsPublic("/store") && !WebsiteRoutes.IsPublic("/login"), "ERP and storefront remain outside the public route surface.");

// Simulate a stored pre-replacement draft/revision with custom ordering, duplicate cards and media.
var legacy = approved.Clone();
foreach (var key in legacy.Fields.Keys.Where(x => x.StartsWith("Approved", StringComparison.Ordinal)).ToList()) legacy.Fields.Remove(key);
foreach (var key in legacy.Collections.Keys.Where(x => x.StartsWith("Approved", StringComparison.Ordinal)).ToList()) legacy.Collections.Remove(key);
foreach (var path in WebsiteServicePages.Sections.Keys) legacy.Pages.Remove(path);
foreach (var sections in legacy.Pages.Values) sections.RemoveAll(x => x.Key.StartsWith("Approved", StringComparison.Ordinal));
legacy.Pages["/"].Reverse();
legacy.Pages["/"][0].Visible = false;
legacy.Fields["WebsiteHeader.f004"].Ar = "هوية محفوظة";
var logo = Guid.NewGuid();
legacy.Fields["WebsiteHeader.logo"].MediaId = logo;
var profile = Guid.NewGuid();
legacy.Fields["MediaCenter.profile"].MediaId = profile;
var image = Guid.NewGuid();
var originalCard = legacy.Collections["FeaturedProjects.collection1"][0];
originalCard.Fields["FeaturedProjects.collection1.item1.f004"] = new() { MediaId = image };
var duplicate = new WebsiteCollectionItem { Id = Guid.NewGuid(), TemplateKey = originalCard.TemplateKey, Original = false,
    Fields = new() { ["FeaturedProjects.collection1.item1.f007"] = new() { Ar = "مشروع معدل", En = "Edited project" } } };
legacy.Collections["FeaturedProjects.collection1"].Insert(0, duplicate);
var before = JsonSerializer.Serialize(legacy);
var restored = JsonSerializer.Deserialize<WebsiteSnapshot>(before)!;
var adapted = WebsiteContentCompatibility.Adapt(restored);
Check(JsonSerializer.Serialize(legacy) == before && JsonSerializer.Serialize(restored) == before, "Compatibility must not rewrite historical input.");
Check(adapted.Fields["WebsiteHeader.f004"].Ar == "هوية محفوظة", "Recognised editorial values survive.");
Check(adapted.Fields["WebsiteHeader.logo"].MediaId == logo, "Logo upload survives.");
Check(adapted.Collections["FeaturedProjects.collection1"][0].Id == duplicate.Id, "Collection IDs and order survive.");
Check(adapted.Collections["FeaturedProjects.collection1"][0].Fields.Values.First().En == "Edited project", "Item overrides survive.");
Check(adapted.Pages["/"].Take(legacy.Pages["/"].Count).Select(x => x.Key).SequenceEqual(legacy.Pages["/"].Select(x => x.Key)), "Page ordering survives.");
Check(!adapted.Pages["/"][0].Visible, "Existing visibility survives.");
Check(adapted.Pages["/"].Where(x => x.Key.StartsWith("Approved", StringComparison.Ordinal)).All(x => !x.Visible), "Added sections remain hidden until explicitly loaded.");
foreach (var path in WebsiteServicePages.Sections.Keys)
    Check(!WebsiteServicePages.IsVisible(adapted, path), "Old publication must not expose a new service: " + path);
var hiddenService = approved.Clone();
hiddenService.Pages["/services/meals"].Single(x => x.Key == "ApprovedMeals").Visible = false;
Check(!WebsiteServicePages.IsVisible(hiddenService, "/services/meals"), "A shared FAQ or closing section must not expose a hidden service.");
Check(adapted.Fields.Keys.ToHashSet().SetEquals(approved.Fields.Keys), "Adapted fields match backend validation shape.");
Check(adapted.Collections.Keys.ToHashSet().SetEquals(approved.Collections.Keys), "Adapted collections match backend validation shape.");
Check(adapted.Pages.Keys.ToHashSet().SetEquals(approved.Pages.Keys), "Adapted pages match backend validation shape.");
Check(JsonSerializer.Serialize(WebsiteContentCompatibility.Adapt(adapted)) == JsonSerializer.Serialize(adapted), "Repeated read/restore adaptation is idempotent.");
var replacement = WebsiteContentCompatibility.ApprovedReplacement(adapted);
Check(replacement.Fields["WebsiteHeader.logo"].MediaId == logo && replacement.Fields["MediaCenter.profile"].MediaId == profile, "Replacement retains branding and resources.");
Check(replacement.Fields["ApprovedFeaturedProjects.collection1.item1.image"].MediaId == image ||
    replacement.Fields["ApprovedProjects.collection1.item5.image"].MediaId == image, "Mapped original card upload survives.");
Check(replacement.Fields["ApprovedContact.email"].Ar == WebsiteWhatsApp.Email, "Correct email overrides draft content.");
Check(replacement.Fields["ApprovedContact.phone"].En == WebsiteWhatsApp.Phone, "Correct phone overrides draft content.");

var message = WebsiteWhatsApp.Message(new Dictionary<string, string> {
    ["الاسم"] = " جمعية & وقف + ",
    ["Services"] = "Iftar, Water",
    ["Package"] = "Quran",
    ["Optional email"] = " ",
    ["Details"] = "Line 1\nLine 2? #100%"
});
var link = WebsiteWhatsApp.Link(message);
Check(link.StartsWith("https://wa.me/966580007040?text=", StringComparison.Ordinal), "Confirmed WhatsApp destination.");
Check(Uri.UnescapeDataString(link[(link.IndexOf("text=", StringComparison.Ordinal) + 5)..]) == message, "Arabic, line breaks and reserved characters round-trip.");
Check(message.Contains("Iftar, Water") && message.Contains("Package: Quran"), "Multiple services and package survive the handoff.");
Check(!message.Contains("Optional email"), "Empty optional values are omitted.");
Check(message.StartsWith("الاسم: جمعية & وقف +", StringComparison.Ordinal), "Form values are trimmed.");
foreach (var path in WebsiteServicePages.Sections.Keys)
    Check(WebsiteWhatsApp.ServiceKey(path.Split('/')[^1]) != null, "Every service route can preselect a quotation.");
Check(WebsiteWhatsApp.ServiceKey("إعاشة حجاج") == "hajj" && WebsiteWhatsApp.ServiceKey("unknown") == null, "Legacy service selection and unknown query values.");
foreach (var field in manifest.Fields.Where(x => x.Key.StartsWith("Approved", StringComparison.Ordinal)))
{
    Check(!field.Ar.Contains("najehah.com.sa") && !field.En.Contains("najehah.com.sa") &&
        !field.Ar.Contains("info@al-afkar.sa") && !field.En.Contains("info@al-afkar.sa"), "Retired contacts must not return: " + field.Key);
}
// Render components in memory: no web server, browser or external calls.
var navigation = new CheckNavigation();
var state = new WebsiteContentState(navigation);
var context = new HttpContextAccessor { HttpContext = new DefaultHttpContext() };
var services = new ServiceCollection().AddLogging()
    .AddSingleton<NavigationManager>(navigation).AddSingleton(state)
    .AddSingleton<IHttpContextAccessor>(context).AddSingleton<IJSRuntime, NoInterop>()
    .AddSingleton<IConfiguration>(new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?> { ["PublicWebsite:Origin"] = "https://alafkarsa.com" }).Build())
    .BuildServiceProvider();
var pages = new Dictionary<string, Type> {
    ["/"] = typeof(Landing), ["/about"] = typeof(About), ["/services"] = typeof(Services),
    ["/projects"] = typeof(Projects), ["/packages"] = typeof(Packages), ["/infrastructure"] = typeof(Infrastructure),
    ["/contact"] = typeof(Contact), ["/request-proposal"] = typeof(RequestProposal),
    ["/privacy-policy"] = typeof(Privacy), ["/terms-and-conditions"] = typeof(Terms)
};
foreach (var path in WebsiteServicePages.Sections.Keys) pages[path] = typeof(WebsiteServiceDetail);
foreach (var english in new[] { false, true })
{
    foreach (var (path, type) in pages)
    {
        navigation.Move(english ? (path == "/" ? "/en" : "/en" + path) : path);
        state.Set(approved);
        await using var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>());
        var html = await renderer.Dispatcher.InvokeAsync(async () =>
            (await renderer.RenderComponentAsync(type, ParameterView.Empty)).ToHtmlString());
        Check(Regex.Matches(html, "<h1(?:\\s|>)").Count == 1, $"Exactly one rendered H1: {navigation.Uri}");
        Check(!html.Contains("najehah.com.sa") && !html.Contains("info@al-afkar.sa") && !html.Contains("000 0000"), "No retired contacts in rendered page: " + path);
        Check(!html.Contains("180,000+%") && !html.Contains("256-bit") && !html.Contains("99.4%"), "No demonstration indicators: " + path);
        foreach (Match attribute in Regex.Matches(html, "data-pw-fields=\"([^\"]+)\""))
            foreach (var key in attribute.Groups[1].Value.Split(','))
                Check(approved.Fields.ContainsKey(key), $"Rendered editor field exists: {path} / {key}");
        if (path == "/request-proposal")
            Check(html.Contains("type=\"checkbox\"") && html.Contains("type=\"email\"") && !Regex.IsMatch(html, "type=\"email\"[^>]*required"), "Quotation uses multiple services and optional email.");
    }
}
navigation.Move("/services/meals");
state.Set(adapted);
await using (var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>()))
{
    var html = await renderer.Dispatcher.InvokeAsync(async () =>
        (await renderer.RenderComponentAsync<WebsiteServiceDetail>()).ToHtmlString());
    Check(context.HttpContext.Response.StatusCode == 404, "Unpublished service returns 404 during prerender.");
    Check(!html.Contains("website-approved-card"), "Unpublished service must not render approved service content.");
}
state.Set(approved);
navigation.Move("/services/meals?service=meals&package=Fruit#details");
await using (var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>()))
{
    var html = await renderer.Dispatcher.InvokeAsync(async () =>
        (await renderer.RenderComponentAsync<WebsiteHeader>()).ToHtmlString());
    Check(html.Contains("/en/services/meals?service=meals&amp;package=Fruit#details"), "Language switching retains query preselection and anchors.");
    Check(html.Contains("href=\"/services\" aria-current=\"page\""), "Services navigation is active on detail pages.");
    Check(!html.Contains("#media-resources"), "Missing media downloads must not leave a dead header link.");
}
navigation.Move("/?websiteEditor=" + Guid.NewGuid().ToString("N"));
Check(state.PreviewRequested, "Preview pages are identifiable before JavaScript connects.");
await services.DisposeAsync();
Console.WriteLine($"Passed {checks} PublicWebsite content, compatibility, route, in-memory rendering and WhatsApp checks.");

sealed class CheckNavigation : NavigationManager
{
    public CheckNavigation() => Initialize("https://alafkarsa.com/", "https://alafkarsa.com/");
    public void Move(string path) => NavigateToCore(path, false);
    protected override void NavigateToCore(string uri, bool forceLoad) { Uri = ToAbsoluteUri(uri).AbsoluteUri; NotifyLocationChanged(false); }
}
sealed class NoInterop : IJSRuntime
{
    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args) => throw new InvalidOperationException("Unexpected JavaScript call during static render.");
    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args) => InvokeAsync<TValue>(identifier, args);
}
