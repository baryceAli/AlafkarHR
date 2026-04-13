using AlAfkarERP.Shared.Dtos.Auth;

namespace AlAfkarERP.Shared.Utilities;

public class TokenService : ITokenService
{
    private AuthTokens? _tokens;
    
    
    public  Task<AuthTokens?> GetTokensAsync()
    {
        return Task.FromResult(_tokens);
    }

    public  Task SetTokensAsync(AuthTokens tokens)
    {

        _tokens = tokens;
        return Task.CompletedTask;
    }

    public  Task ClearTokensAsync()
    {
        _tokens = null;
        return Task.CompletedTask;
    }
}