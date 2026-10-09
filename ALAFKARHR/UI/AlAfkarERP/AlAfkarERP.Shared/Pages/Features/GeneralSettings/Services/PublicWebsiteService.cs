using System.Net.Http.Json;
using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using AlAfkarERP.Shared.Utilities;
using SharedWithUI.GeneralSettings.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.GeneralSettings.Services;

public interface IWebsitePreviewSessions
{
    void Register(string id, string accessToken, IEnumerable<Guid> mediaIds);
    void Remove(string id);
}

public sealed class PublicWebsiteService(HttpClient http, ITokenService tokens, ApiConfig config) : BaseApiService(http, tokens, config)
{
    private readonly string path = $"api/{config.Version}/publicwebsite";
    public Task<ApiResult<WebsiteConfigurationDto>> ConfigurationAsync() => SendAsync<WebsiteConfigurationDto>(new(HttpMethod.Get, path + "/configuration"), null);
    public Task<ApiResult<WebsiteStorageChoice>> CreateStorageFolderAsync(WebsiteStorageFolderRequest request) => SendAsync<WebsiteStorageChoice>(new(HttpMethod.Post, path + "/configuration/storage-locations") { Content = JsonContent.Create(request) }, null);
    public Task<ApiResult<WebsiteConfigurationDto>> SaveConfigurationAsync(WebsiteConfigurationSaveRequest request) => SendAsync<WebsiteConfigurationDto>(new(HttpMethod.Put, path + "/configuration") { Content = JsonContent.Create(request) }, null);
    public Task<ApiResult<WebsiteReadinessDto>> CheckReadinessAsync(WebsiteConfigurationSaveRequest request) => SendAsync<WebsiteReadinessDto>(new(HttpMethod.Post, path + "/configuration/readiness") { Content = JsonContent.Create(request) }, null);
    public Task<ApiResult<WebsiteConfigurationDto>> ActivateAsync(Guid token) => SendAsync<WebsiteConfigurationDto>(new(HttpMethod.Post, path + "/configuration/activate") { Content = JsonContent.Create(new WebsiteVersionRequest(token)) }, null);
    public Task<ApiResult<WebsiteManagementStatusDto>> ManagementStatusAsync() => SendAsync<WebsiteManagementStatusDto>(new(HttpMethod.Get, path + "/management-status"), null);
    public async Task<string?> PreviewTokenAsync()
    {
        var auth = await tokens.GetTokensAsync();
        if (auth == null) return null;
        var jwt = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().ReadJwtToken(auth.AccessToken);
        if (jwt.ValidTo <= DateTime.UtcNow.AddMinutes(1))
        {
            if (!await tokens.RefreshTokensAsync(http, $"api/{config.Version}/auth/refresh-token", auth.AccessToken)) return null;
            auth = await tokens.GetTokensAsync();
        }
        return auth?.AccessToken;
    }
    public async Task<WebsiteSnapshot> PublishedAsync(CancellationToken ct = default)
    {
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(ct);
        timeout.CancelAfter(TimeSpan.FromSeconds(10));
        using var response = await http.GetAsync(path + "/published", timeout.Token);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<WebsiteSnapshot>(timeout.Token))!;
    }
    public Task<ApiResult<WebsiteDraftDto>> DraftAsync() => SendAsync<WebsiteDraftDto>(new(HttpMethod.Get, path + "/draft"), null);
    public Task<ApiResult<WebsiteDraftDto>> SaveAsync(WebsiteDraftDto draft) => SendAsync<WebsiteDraftDto>(new(HttpMethod.Put, path + "/draft") { Content = JsonContent.Create(new WebsiteSaveRequest(draft.Token, draft.Content)) }, null);
    public Task<ApiResult<WebsiteDraftDto>> PublishAsync(Guid token) => SendAsync<WebsiteDraftDto>(new(HttpMethod.Post, path + "/publish") { Content = JsonContent.Create(new WebsiteVersionRequest(token)) }, null);
    public Task<ApiResult<List<WebsiteRevisionDto>>> RevisionsAsync() => SendAsync<List<WebsiteRevisionDto>>(new(HttpMethod.Get, path + "/revisions"), null);
    public Task<ApiResult<WebsiteDraftDto>> RestoreAsync(Guid revision, Guid token) => SendAsync<WebsiteDraftDto>(new(HttpMethod.Post, $"{path}/revisions/{revision}/restore") { Content = JsonContent.Create(new WebsiteVersionRequest(token)) }, null);
    public async Task<ApiResult<WebsiteMediaDto>> UploadAsync(Stream stream, string name, string contentType, IProgress<int>? progress = null)
    {
        // Browsers can report platform-specific MIME aliases; send the canonical supported type.
        var canonicalType = Path.GetExtension(name).ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg", ".png" => "image/png", ".gif" => "image/gif", ".webp" => "image/webp",
            ".pdf" => "application/pdf", ".mp4" => "video/mp4", ".webm" => "video/webm", ".mp3" => "audio/mpeg", ".zip" => "application/zip", _ => contentType
        };
        using var form = new MultipartFormDataContent();
        using var content = new StreamContent(new UploadProgressStream(stream, progress));
        content.Headers.ContentType = new(canonicalType); form.Add(content, "file", name);
        using var request = new HttpRequestMessage(HttpMethod.Post, path + "/media") { Content = form };
        var token = await PreviewTokenAsync();
        if (token != null) request.Headers.Authorization = new("Bearer", token);
        using var response = await http.SendAsync(request);
        if (!response.IsSuccessStatusCode)
            return ApiResult<WebsiteMediaDto>.Failure(ApiErrorFormatter.FromHttpError(response.StatusCode, await response.Content.ReadAsStringAsync()));
        return ApiResult<WebsiteMediaDto>.Success((await response.Content.ReadFromJsonAsync<WebsiteMediaDto>())!);
    }
}

internal sealed class UploadProgressStream(Stream inner, IProgress<int>? progress) : Stream
{
    private long read;
    public override bool CanRead => inner.CanRead;
    public override bool CanSeek => false;
    public override bool CanWrite => false;
    public override long Length => inner.Length;
    public override long Position { get => read; set => throw new NotSupportedException(); }
    public override int Read(byte[] buffer, int offset, int count) { var n = inner.Read(buffer, offset, count); Report(n); return n; }
    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken ct = default) { var n = await inner.ReadAsync(buffer, ct); Report(n); return n; }
    private void Report(int count) { read += count; progress?.Report((int)Math.Min(100, read * 100 / Math.Max(1, Length))); }
    public override void Flush() => throw new NotSupportedException();
    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    public override void SetLength(long value) => throw new NotSupportedException();
    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
}
