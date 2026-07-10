using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Dtos.Auth;
using AlAfkarERP.Shared.Utilities;
using Microsoft.AspNetCore.Components.Authorization;
using SharedWithUI.Auth.Dtos;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.Auth.Services;

public class AuthService : AuthBaseApiService, IAuthService
{
    //private readonly HttpClient _http;
    private readonly string _path;
    private readonly ITokenService _tokenService;
    private readonly ApiConfig _apiConfig;
    private readonly CustomAuthStateProvider _authStateProvider;

    public AuthService(HttpClient http,
        ITokenService tokenService,
        AuthenticationStateProvider authStateProvider,
        ApiConfig apiConfig) : base(http)
    {
        //_http = http;
        _tokenService = tokenService;
        _apiConfig = apiConfig;
        _path = $"{_apiConfig.BaseURL}/api/{_apiConfig.Version}/auth";
        _authStateProvider = (CustomAuthStateProvider)authStateProvider;

    }
    public async Task<ApiResult<LoginResponseDto>> LoginAsync(string email, string password, bool rememberDevice)
    {
        //var 
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/login")
        {
            Content = JsonContent.Create(new
            {
                Login = new LoginDto { Email = email, Password = password }
            })
        };

        var response = await SendAsync<LoginResponseDto>(request, "login");

        if (response.IsSuccess)
        {
            AuthTokens tokens = new AuthTokens
            {
                AccessToken = response.Data.AccessToken,
                RefreshToken = response.Data.RefreshToken
            };
            await _tokenService.SetTokensAsync(tokens, rememberDevice);

            _authStateProvider.NotifyUserAuthentication(tokens.AccessToken);
        }
        return response;
    }

    public async Task<ApiResult<Guid>> RegisterAsync(RegisterDto register)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/register")
        {
            Content = JsonContent.Create(new
            {
                Register = register
            })
        };

        return await SendAsync<Guid>(request, "id");
    }

    public async Task<ApiResult<bool>> GenerateResetPasswordOtpAsync(string userIdentifier)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/reset-password")
        {
            Content = JsonContent.Create(new
            {
                UserIdentifier = userIdentifier
            })
        };

        return await SendAsync<bool>(request, "isSuccess");
    }

    public async Task<ApiResult<bool>> ConfirmOtpAsync(string userIdentifier, string otp)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/confirm-otp")
        {
            Content = JsonContent.Create(new
            {
                UserIdentifier = userIdentifier,
                OTP = otp
            })
        };

        return await SendAsync<bool>(request, "isConfirmed");
    }
    
    public async Task<bool> RefreshTokenAsync()
    {
        var tokens = await _tokenService.GetTokensAsync();
        if (tokens == null) return false;

        return await _tokenService.RefreshTokensAsync(
            _http,
            $"{_path}/refresh-token",
            tokens.AccessToken,
            accessToken =>
            {
                _authStateProvider.NotifyUserAuthentication(accessToken);
                return Task.CompletedTask;
            },
            _authStateProvider.NotifyUserLogoutStateChanged);
    }

    public async Task LogoutAsync()
    {
        await _tokenService.ClearTokensAsync();
        _authStateProvider.NotifyUserLogoutStateChanged();
    }

    
    public async Task<ApiResult<PaginatedResult<UserDto>>> GetUsersAsync(Guid companyId,int pageIndex, int pageSize)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/company/{companyId}/users");
        return await SendAsync<PaginatedResult<UserDto>>(request, "userList");
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

    public Task<ApiResult<UserDto>> GetUserById(Guid Id)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResult<UserDto>> GetUserByEmployeeId(Guid employeeId)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResult<bool>> AssignUserToRole()
    {
        throw new NotImplementedException();
    }
}
