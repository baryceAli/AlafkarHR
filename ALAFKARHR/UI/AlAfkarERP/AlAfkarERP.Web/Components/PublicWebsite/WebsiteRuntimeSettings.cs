using System.Net.Http.Json;
using AlAfkarERP.Shared.Dtos;
using SharedWithUI.GeneralSettings.Dtos;

namespace AlAfkarERP.Web.Components.PublicWebsite;

// Only anonymous runtime settings enter this process-wide fallback; never private setup details.
public sealed class WebsiteRuntimeSettings(IHttpClientFactory clients, ApiConfig api, IConfiguration configuration, ILogger<WebsiteRuntimeSettings> logger)
{
    private string? lastOrigin;
    public async Task<string> OriginAsync(CancellationToken ct = default)
    {
        try
        {
            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(ct);
            timeout.CancelAfter(TimeSpan.FromSeconds(10));
            var result = await clients.CreateClient("AlAfkarERP").GetFromJsonAsync<WebsiteRuntimeSettingsDto>($"api/{api.Version}/publicwebsite/runtime-settings", timeout.Token);
            if (result != null && Uri.TryCreate(result.PublicOrigin, UriKind.Absolute, out var uri) && uri.Scheme is "http" or "https" && uri.AbsolutePath == "/" && uri.UserInfo.Length == 0 && uri.Query.Length == 0 && uri.Fragment.Length == 0)
            {
                var origin = uri.GetLeftPart(UriPartial.Authority); Volatile.Write(ref lastOrigin, origin); return origin;
            }
        }
        catch (Exception e) when (e is HttpRequestException or OperationCanceledException or System.Text.Json.JsonException)
        { logger.LogWarning(e, "Unable to load public website runtime settings."); }
        return Volatile.Read(ref lastOrigin) ?? WebsiteRoutes.Origin(configuration);
    }
}
