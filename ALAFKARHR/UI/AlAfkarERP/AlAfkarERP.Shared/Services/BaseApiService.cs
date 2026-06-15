
using AlAfkarERP.Shared.Dtos;
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

            // ❌ NOT success
            if (!response.IsSuccessStatusCode)
            {
                ErrorResponseDto? error = null;

                try
                {
                    error = DeserializeAPIResponse.Deserialize<ErrorResponseDto>(content,node);
                }
                catch
                {
                    error = new ErrorResponseDto
                    {
                        Status = (int)response.StatusCode,
                        Title = "Request failed",
                        Detail = content
                    };
                }

                return ApiResult<T>.Failure(error!);
            }

            // ✅ success
            var result = DeserializeAPIResponse.Deserialize<T>(content,node);

            return ApiResult<T>.Success(result);
        }
        catch (Exception ex)
        {
            return ApiResult<T>.Failure(new ErrorResponseDto
            {
                Status = 500,
                Title = "Client Error",
                Detail = ex.Message
            });
        }
    }
}
