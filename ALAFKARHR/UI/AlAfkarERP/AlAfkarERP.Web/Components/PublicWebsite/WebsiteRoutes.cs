namespace AlAfkarERP.Web.Components.PublicWebsite;
public static class WebsiteRoutes
{
 public static readonly string[] Paths = ["/", "/services", "/packages", "/projects", "/about", "/infrastructure", "/contact", "/request-proposal", "/privacy-policy", "/terms-and-conditions"];
 public static string ArabicPath(string? path)
 {
  var normalized = path?.TrimEnd('/') is "" ? "/" : path?.TrimEnd('/') ?? "/";
  return normalized.Equals("/en", StringComparison.OrdinalIgnoreCase) ? "/" : normalized.StartsWith("/en/", StringComparison.OrdinalIgnoreCase) ? normalized[3..] : normalized;
 }
 public static bool IsPublic(string? path) => path != null && (Paths.Contains(ArabicPath(path), StringComparer.OrdinalIgnoreCase) || ArabicPath(path) == "/website-not-found");
 public static string Origin(IConfiguration configuration)
 {
  var origin = configuration["PublicWebsite:Origin"] ?? configuration["ApiConfig:WebSiteURL"];
  if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri) || uri.Scheme is not ("https" or "http") || uri.AbsolutePath != "/")
   throw new InvalidOperationException("Configure PublicWebsite:Origin with the public HTTP(S) origin.");
  return uri.GetLeftPart(UriPartial.Authority);
 }
}
