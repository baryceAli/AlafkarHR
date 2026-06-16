using AlAfkarERP.Shared.Dtos.Auth;
using Microsoft.JSInterop;
using System.Text.Json;

namespace AlAfkarERP.Shared.Utilities;

public class TokenService(IJSRuntime jsRuntime) : ITokenService
{
    private const string StorageKey = "alafkarerp.authTokens";

    private AuthTokens? _tokens;
    private bool _rememberDevice;

    public async Task<AuthTokens?> GetTokensAsync()
    {
        if (_tokens != null)
        {
            return _tokens;
        }

        try
        {
            var json = await jsRuntime.InvokeAsync<string?>("localStorage.getItem", StorageKey);
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            var tokens = JsonSerializer.Deserialize<AuthTokens>(json);
            if (tokens == null ||
                string.IsNullOrWhiteSpace(tokens.AccessToken) ||
                string.IsNullOrWhiteSpace(tokens.RefreshToken))
            {
                await ClearTokensAsync();
                return null;
            }

            _tokens = tokens;
            _rememberDevice = true;
            return _tokens;
        }
        catch (InvalidOperationException)
        {
            return null;
        }
        catch (JSException)
        {
            return null;
        }
        catch (JsonException)
        {
            await ClearTokensAsync();
            return null;
        }
    }

    public async Task SetTokensAsync(AuthTokens tokens, bool? rememberDevice = null)
    {
        _tokens = tokens;
        _rememberDevice = rememberDevice ?? _rememberDevice;

        try
        {
            if (_rememberDevice)
            {
                var json = JsonSerializer.Serialize(tokens);
                await jsRuntime.InvokeVoidAsync("localStorage.setItem", StorageKey, json);
            }
            else
            {
                await jsRuntime.InvokeVoidAsync("localStorage.removeItem", StorageKey);
            }
        }
        catch (InvalidOperationException)
        {
        }
        catch (JSException)
        {
        }
    }

    public async Task ClearTokensAsync()
    {
        _tokens = null;
        _rememberDevice = false;

        try
        {
            await jsRuntime.InvokeVoidAsync("localStorage.removeItem", StorageKey);
        }
        catch (InvalidOperationException)
        {
        }
        catch (JSException)
        {
        }
    }
}
