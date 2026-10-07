using System.Collections.Concurrent;
using AlAfkarERP.Shared.Dtos;

namespace AlAfkarERP.Web.Components.PublicWebsite;

// Short-lived capabilities expose only media IDs in an authorized editor's draft.
// ERP credentials stay on the server and are never passed to the preview frame.
public sealed class WebsitePreviewSessions : AlAfkarERP.Shared.Pages.Features.GeneralSettings.Services.IWebsitePreviewSessions
{
    public sealed record Session(string AccessToken, HashSet<Guid> MediaIds, DateTime ExpiresAt);
    private readonly ConcurrentDictionary<string, Session> sessions = new();
    public void Register(string id, string accessToken, IEnumerable<Guid> mediaIds)
    {
        foreach (var entry in sessions.Where(x => x.Value.ExpiresAt < DateTime.UtcNow)) sessions.TryRemove(entry.Key, out _);
        sessions[id] = new(accessToken, mediaIds.ToHashSet(), DateTime.UtcNow.AddMinutes(30));
    }
    public Session? Get(string id, Guid mediaId) => sessions.TryGetValue(id, out var s) && s.ExpiresAt > DateTime.UtcNow && s.MediaIds.Contains(mediaId) ? s : null;
    public void Remove(string id) => sessions.TryRemove(id, out _);
}

public static class WebsiteMediaProxy
{
    public static void Map(WebApplication app, ApiConfig config)
    {
        app.MapGet("/website/media/{id:guid}", async (Guid id, IHttpClientFactory clients, HttpContext context) =>
            await Forward(context, clients, $"api/{config.Version}/publicwebsite/media/{id}", null));
        app.MapGet("/publicwebsite-preview-media/{session}/{id:guid}", async (string session, Guid id,
            WebsitePreviewSessions sessions, IHttpClientFactory clients, HttpContext context) =>
        {
            var capability = sessions.Get(session, id);
            if (capability == null) { context.Response.StatusCode = 404; return; }
            await Forward(context, clients, $"api/{config.Version}/publicwebsite/media/{id}/draft", capability.AccessToken);
        });
    }
    private static async Task Forward(HttpContext context, IHttpClientFactory clients, string path, string? token)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, path);
        if (token != null) request.Headers.Authorization = new("Bearer", token);
        if (context.Request.Headers.TryGetValue("Range", out var range)) request.Headers.TryAddWithoutValidation("Range", range.ToString());
        using var response = await clients.CreateClient("AlAfkarERP").SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.RequestAborted);
        context.Response.StatusCode = (int)response.StatusCode;
        context.Response.Headers.CacheControl = token != null || !response.IsSuccessStatusCode ? "private, no-store" : "public, max-age=31536000, immutable";
        context.Response.Headers["X-Content-Type-Options"] = "nosniff";
        if (token != null) context.Response.Headers["X-Robots-Tag"] = "noindex, nofollow";
        if (response.Content.Headers.ContentType != null) context.Response.ContentType = response.Content.Headers.ContentType.ToString();
        if (response.Content.Headers.ContentLength is long length) context.Response.ContentLength = length;
        if (response.Content.Headers.ContentRange != null) context.Response.Headers.ContentRange = response.Content.Headers.ContentRange.ToString();
        if (response.Content.Headers.ContentDisposition != null) context.Response.Headers.ContentDisposition = response.Content.Headers.ContentDisposition.ToString();
        if (response.Headers.AcceptRanges.Any()) context.Response.Headers.AcceptRanges = "bytes";
        await response.Content.CopyToAsync(context.Response.Body, context.RequestAborted);
    }
}
