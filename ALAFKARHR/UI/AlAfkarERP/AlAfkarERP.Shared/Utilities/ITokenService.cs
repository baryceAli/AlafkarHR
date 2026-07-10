
using AlAfkarERP.Shared.Dtos.Auth;

namespace AlAfkarERP.Shared.Utilities;

public interface ITokenService
{
    Task<AuthTokens?> GetTokensAsync();
    Task SetTokensAsync(AuthTokens tokens, bool? rememberDevice = null);
    Task ClearTokensAsync();
    Task<bool> ClearTokensIfRefreshTokenMatchesAsync(string refreshToken);
    Task<bool> RefreshTokensAsync(
        HttpClient http,
        string refreshUrl,
        string? attemptedAccessToken = null,
        Func<string, Task>? onAuthenticated = null,
        Action? onLoggedOut = null);
}
