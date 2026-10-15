using Microsoft.AspNetCore.Http.Extensions;

namespace AlAfkarERP.Web.Components.PublicWebsite;

public static class WebsiteRoutes
{
    public static readonly string[] Paths = ["/", "/about", "/activities", "/services", "/projects", "/partners", "/contact", "/request-proposal"];
    private static readonly Dictionary<string, string> ServiceRedirects = new(StringComparer.OrdinalIgnoreCase)
    {
        ["/services/iftar-haram"] = "/services#iftar",
        ["/services/suqya"] = "/services#suqya",
        ["/services/meals"] = "/services#meals",
        ["/services/food-baskets"] = "/services#baskets",
        ["/services/hajj-season"] = "/services#hajj",
        ["/infrastructure"] = "/services#infrastructure"
    };
    private static readonly string[] Retired = ["/packages", "/privacy-policy", "/terms-and-conditions"];
    private static readonly HashSet<string> EditorParameters = new(StringComparer.OrdinalIgnoreCase)
        { "websiteEditor", "websitePreview", "previewSession", "editorSession" };

    public static string ArabicPath(string? path)
    {
        var normalized = string.IsNullOrEmpty(path?.TrimEnd('/')) ? "/" : path.TrimEnd('/');
        return normalized.Equals("/en", StringComparison.OrdinalIgnoreCase) ? "/"
            : normalized.StartsWith("/en/", StringComparison.OrdinalIgnoreCase) ? normalized[3..] : normalized;
    }

    public readonly record struct Migration(int Status = 0, string? Destination = null);
    public static Migration Resolve(string? path)
    {
        if (path == null) return default;
        var normalized = ArabicPath(path);
        if (path.Equals("/design-2", StringComparison.OrdinalIgnoreCase) || path.StartsWith("/design-2/", StringComparison.OrdinalIgnoreCase))
        {
            var selected = "/" + path[9..].Trim('/');
            var canonical = Paths.FirstOrDefault(p => p.Equals(selected, StringComparison.OrdinalIgnoreCase));
            return canonical == null ? new(404) : new(301, canonical);
        }
        if (Retired.Contains(normalized, StringComparer.OrdinalIgnoreCase)) return new(410);
        if (normalized.Equals("/services/pilgrim-housing", StringComparison.OrdinalIgnoreCase)) return new(404);
        if (ServiceRedirects.TryGetValue(normalized, out var target)) return new(301, target);
        var known = Paths.FirstOrDefault(p => p.Equals(normalized, StringComparison.OrdinalIgnoreCase));
        return known != null && !path.Equals(known, StringComparison.Ordinal) ? new(301, known) : default;
    }

    public static bool HasEditorParameters(IQueryCollection query) => query.Keys.Any(EditorParameters.Contains);
    public static string WithQuery(string destination, IQueryCollection query)
    {
        var split = destination.Split('#', 2);
        var builder = new QueryBuilder();
        foreach (var pair in query.Where(p => !EditorParameters.Contains(p.Key)))
            foreach (var value in pair.Value) builder.Add(pair.Key, value ?? "");
        return split[0] + builder.ToQueryString() + (split.Length == 2 ? "#" + split[1] : "");
    }

    // Explicit shell selection; unmatched endpoints use the existing status-page mechanism.
    public static bool IsPublic(string? path) => path != null &&
        (Paths.Contains(ArabicPath(path), StringComparer.OrdinalIgnoreCase) || path.Equals("/website-not-found", StringComparison.OrdinalIgnoreCase));

    public static string Origin(IConfiguration configuration)
    {
        var origin = configuration["PublicWebsite:Origin"] ?? configuration["ApiConfig:WebSiteURL"];
        if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri) || uri.Scheme is not ("https" or "http") || uri.AbsolutePath != "/" || uri.UserInfo.Length != 0 || uri.Query.Length != 0 || uri.Fragment.Length != 0)
            throw new InvalidOperationException("Configure PublicWebsite:Origin with the public HTTP(S) origin.");
        return uri.GetLeftPart(UriPartial.Authority);
    }
}
