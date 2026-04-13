
using AlAfkarERP.Shared.Dtos.Auth;

namespace AlAfkarERP.Shared.Utilities;

public interface ITokenService
{
    Task<AuthTokens?> GetTokensAsync();
    Task SetTokensAsync(AuthTokens tokens);
    Task ClearTokensAsync();
}