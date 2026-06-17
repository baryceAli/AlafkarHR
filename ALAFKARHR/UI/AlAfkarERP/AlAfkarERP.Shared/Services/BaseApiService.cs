using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Dtos.Auth;
using AlAfkarERP.Shared.Utilities;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Services;

public abstract class BaseApiService
{
    protected readonly HttpClient _http;
    private readonly ITokenService _tokenService;
    private readonly ApiConfig _apiConfig;
    private static readonly SemaphoreSlim RefreshLock = new(1, 1);

    protected BaseApiService(HttpClient http, ITokenService tokenService, ApiConfig apiConfig)
    {
        _http = http;
        _tokenService = tokenService;
        _apiConfig = apiConfig;
    }

    protected async Task<ApiResult<T>> SendAsync<T>(HttpRequestMessage request, string? node)
    {
        try
        {
            var tokens = await _tokenService.GetTokensAsync();
            ApplyAuthorizationHeader(request, tokens);

            var retryRequest = await CloneHttpRequestMessageAsync(request);
            var response = await _http.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.Unauthorized &&
                tokens != null &&
                !string.IsNullOrWhiteSpace(tokens.RefreshToken) &&
                await TryRefreshTokenAsync())
            {
                response.Dispose();

                var refreshedTokens = await _tokenService.GetTokensAsync();
                ApplyAuthorizationHeader(retryRequest, refreshedTokens);

                response = await _http.SendAsync(retryRequest);
                content = await response.Content.ReadAsStringAsync();
                retryRequest.Dispose();
            }
            else
            {
                retryRequest.Dispose();
            }

            if (!response.IsSuccessStatusCode)
            {
                return ApiResult<T>.Failure(ApiErrorFormatter.FromHttpError(response.StatusCode, content));
            }

            var result = DeserializeAPIResponse.Deserialize<T>(content, node);

            return ApiResult<T>.Success(result);
        }
        catch (Exception ex)
        {
            return ApiResult<T>.Failure(ApiErrorFormatter.FromClientException(ex));
        }
    }

    private static void ApplyAuthorizationHeader(HttpRequestMessage request, AuthTokens? tokens)
    {
        if (tokens == null || string.IsNullOrWhiteSpace(tokens.AccessToken))
            return;

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", tokens.AccessToken);
    }

    private async Task<bool> TryRefreshTokenAsync()
    {
        await RefreshLock.WaitAsync();
        try
        {
            var tokens = await _tokenService.GetTokensAsync();
            if (tokens == null || string.IsNullOrWhiteSpace(tokens.RefreshToken))
                return false;

            var response = await _http.PostAsJsonAsync($"api/{_apiConfig.Version}/auth/refresh-token", new
            {
                refreshToken = tokens.RefreshToken
            });

            if (!response.IsSuccessStatusCode)
            {
                await _tokenService.ClearTokensAsync();
                return false;
            }

            var newTokens = await response.Content.ReadFromJsonAsync<AuthTokens>();
            if (newTokens == null ||
                string.IsNullOrWhiteSpace(newTokens.AccessToken) ||
                string.IsNullOrWhiteSpace(newTokens.RefreshToken))
            {
                await _tokenService.ClearTokensAsync();
                return false;
            }

            await _tokenService.SetTokensAsync(newTokens);
            return true;
        }
        finally
        {
            RefreshLock.Release();
        }
    }

    private static async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(HttpRequestMessage request)
    {
        var clone = new HttpRequestMessage(request.Method, request.RequestUri)
        {
            Version = request.Version,
            VersionPolicy = request.VersionPolicy
        };

        foreach (var header in request.Headers)
        {
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        foreach (var option in request.Options)
        {
            clone.Options.TryAdd(option.Key, option.Value);
        }

        if (request.Content != null)
        {
            var ms = new MemoryStream();
            await request.Content.CopyToAsync(ms);
            ms.Position = 0;
            clone.Content = new StreamContent(ms);

            foreach (var header in request.Content.Headers)
            {
                clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        return clone;
    }
}
