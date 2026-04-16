using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Dtos.Auth;
using AlAfkarERP.Shared.Pages.Features.Auth.Dtos;
using AlAfkarERP.Shared.Utilities;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using System.Text.Json;
using System.Xml.Linq;

namespace AlAfkarERP.Shared.Services.Auth;

public class AuthService : IAuthService
{
    private readonly HttpClient _http;
    private readonly ITokenService _tokenService;
    private readonly ApiConfig _apiConfigOptions;
    private readonly CustomAuthStateProvider _authStateProvider;

    public AuthService(HttpClient http,
        ITokenService tokenService,
        AuthenticationStateProvider authStateProvider,
        //ApiConfig apiConfig,
        ApiConfig apiConfigOptions)
    {
        _http = http;
        _tokenService = tokenService;
        _apiConfigOptions = apiConfigOptions;
        _authStateProvider = (CustomAuthStateProvider)authStateProvider;
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        var response = await _http.PostAsJsonAsync($"{_apiConfigOptions.BaseURL}/api/{_apiConfigOptions.Version}/auth/login", new { email, password });

        if (!response.IsSuccessStatusCode) return false;

        var tokens = await response.Content.ReadFromJsonAsync<AuthTokens>();

        await _tokenService.SetTokensAsync(tokens);

        _authStateProvider.NotifyUserAuthentication(tokens.AccessToken);

        return true;
    }

    // 🔁 Refresh Token Rotation
    public async Task<bool> RefreshTokenAsync()
    {
        var tokens = await _tokenService.GetTokensAsync();
        if (tokens == null) return false;

        var response = await _http.PostAsJsonAsync($"{_apiConfigOptions.BaseURL}/api{_apiConfigOptions.Version}/auth/refresh", new
        {
            accessToken = tokens.AccessToken,
            refreshToken = tokens.RefreshToken
        });

        if (!response.IsSuccessStatusCode)
        {
            await LogoutAsync();
            return false;
        }

        var newTokens = await response.Content.ReadFromJsonAsync<AuthTokens>();

        // 🔥 ROTATION: overwrite old refresh token
        await _tokenService.SetTokensAsync(newTokens);

        _authStateProvider.NotifyUserAuthentication(newTokens.AccessToken);

        return true;
    }

    public async Task LogoutAsync()
    {
        await _tokenService.ClearTokensAsync();
        _authStateProvider.NotifyUserLogout();
    }

    public async Task<ApiResult<PaginatedResult<UserDto>>> GetAsync(int pageIndex, int pageSize)
    {
        var request = new HttpRequestMessage(HttpMethod.Get,$"{_apiConfigOptions.BaseURL}/api/{_apiConfigOptions.Version}/auth/users");
        try
        {
            var response = await _http.SendAsync(request);

            var content = await response.Content.ReadAsStringAsync();

            // ❌ NOT success
            if (!response.IsSuccessStatusCode)
            {
                ErrorResponseDto? error = null;

                try
                {
                    error = DeserializeAPIResponse.Deserialize<ErrorResponseDto>(content, "userList");
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

                return ApiResult<PaginatedResult<UserDto>>.Failure(error!);
            }

            // ✅ success
            var result = DeserializeAPIResponse.Deserialize<PaginatedResult<UserDto>>(content, "userList");

            return ApiResult<PaginatedResult<UserDto>>.Success(result);
        }
        catch (Exception ex)
        {
            return ApiResult<PaginatedResult<UserDto>>.Failure(new ErrorResponseDto
            {
                Status = 500,
                Title = "Client Error",
                Detail = ex.Message
            });
        }

    }
}