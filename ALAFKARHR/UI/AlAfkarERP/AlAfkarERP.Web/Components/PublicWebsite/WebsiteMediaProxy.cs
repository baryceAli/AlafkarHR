using AlAfkarERP.Shared.Dtos;

namespace AlAfkarERP.Web.Components.PublicWebsite;

// Preserve published CMS media URLs even though the selected website uses static content.
public static class WebsiteMediaProxy
{
    public static void Map(WebApplication app, ApiConfig config)
    {
        app.MapGet("/website/media/{id:guid}", async (Guid id, IHttpClientFactory clients, HttpContext context) =>
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, $"api/{config.Version}/publicwebsite/media/{id}");
            if (context.Request.Headers.TryGetValue("Range", out var range)) request.Headers.TryAddWithoutValidation("Range", range.ToString());
            using var response = await clients.CreateClient("AlAfkarERP").SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.RequestAborted);
            context.Response.StatusCode = (int)response.StatusCode;
            context.Response.Headers.CacheControl = !response.IsSuccessStatusCode ? "private, no-store" : "public, max-age=31536000, immutable";
            context.Response.Headers["X-Content-Type-Options"] = "nosniff";
            if (response.Content.Headers.ContentType != null) context.Response.ContentType = response.Content.Headers.ContentType.ToString();
            if (response.Content.Headers.ContentLength is long length) context.Response.ContentLength = length;
            if (response.Content.Headers.ContentRange != null) context.Response.Headers.ContentRange = response.Content.Headers.ContentRange.ToString();
            if (response.Content.Headers.ContentDisposition != null) context.Response.Headers.ContentDisposition = response.Content.Headers.ContentDisposition.ToString();
            if (response.Headers.AcceptRanges.Any()) context.Response.Headers.AcceptRanges = "bytes";
            await response.Content.CopyToAsync(context.Response.Body, context.RequestAborted);
        });
    }
}
