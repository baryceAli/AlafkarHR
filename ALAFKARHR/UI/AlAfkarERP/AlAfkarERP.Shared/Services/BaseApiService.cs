using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Utilities;
using System.Net.Http.Headers;

namespace AlAfkarERP.Shared.Services;

public abstract class BaseApiService
{
    protected readonly HttpClient _http;
    private readonly ITokenService _tokenService;

    protected BaseApiService(HttpClient http, ITokenService tokenService)
    {
        _http = http;
        _tokenService = tokenService;
    }

    protected async Task<ApiResult<T>> SendAsync<T>(HttpRequestMessage request, string? node)
    {
        try
        {
            var tokens = await _tokenService.GetTokensAsync();
            if (tokens != null && !string.IsNullOrWhiteSpace(tokens.AccessToken))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", tokens.AccessToken);
            }

            var response = await _http.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

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
}
