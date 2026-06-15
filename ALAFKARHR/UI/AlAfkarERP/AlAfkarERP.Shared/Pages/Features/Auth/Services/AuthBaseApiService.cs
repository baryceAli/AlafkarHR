
using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Utilities;

namespace AlAfkarERP.Shared.Pages.Features.Auth.Services;
public abstract class AuthBaseApiService
{
    protected readonly HttpClient _http;

    protected AuthBaseApiService(HttpClient http)
    {
        _http = http;
    }

    protected async Task<ApiResult<T>> SendAsync<T>(HttpRequestMessage request, string? node)
    {
        try
        {
            var response = await _http.SendAsync(request);

            var content = await response.Content.ReadAsStringAsync();

            // ❌ NOT success
            if (!response.IsSuccessStatusCode)
            {
                return ApiResult<T>.Failure(ApiErrorFormatter.FromHttpError(response.StatusCode, content));
            }

            // ✅ success
            var result = DeserializeAPIResponse.Deserialize<T>(content,node);

            return ApiResult<T>.Success(result);
        }
        catch (Exception ex)
        {
            return ApiResult<T>.Failure(ApiErrorFormatter.FromClientException(ex));
        }
    }
}
