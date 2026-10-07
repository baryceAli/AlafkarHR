namespace AlAfkarERP.Web.Components.PublicWebsite;
public static class WebsiteRoutes
{
 public static readonly string[] Paths = ["/", "/services", "/packages", "/projects", "/about", "/infrastructure", "/contact", "/request-proposal", "/privacy-policy", "/terms-and-conditions"];
 public static bool IsPublic(string? path) => Paths.Contains(path?.TrimEnd('/') is "" ? "/" : path?.TrimEnd('/'), StringComparer.OrdinalIgnoreCase) || path == "/website-not-found";
 public static string Origin(IConfiguration configuration)
 {
  var origin = configuration["PublicWebsite:Origin"] ?? configuration["ApiConfig:WebSiteURL"];
  if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri) || uri.Scheme is not ("https" or "http") || uri.AbsolutePath != "/")
   throw new InvalidOperationException("Configure PublicWebsite:Origin with the public HTTP(S) origin.");
  return uri.GetLeftPart(UriPartial.Authority);
 }
}
