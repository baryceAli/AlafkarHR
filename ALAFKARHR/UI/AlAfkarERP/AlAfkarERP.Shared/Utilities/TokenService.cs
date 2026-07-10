using AlAfkarERP.Shared.Dtos.Auth;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using System.Text.Json;

namespace AlAfkarERP.Shared.Utilities;

public class TokenService(IJSRuntime jsRuntime) : ITokenService
{
    private const string StorageKey = "alafkarerp.authTokens";
    private const string LocalStorage = "localStorage";
    private const string SessionStorage = "sessionStorage";

    private AuthTokens? _tokens;
    private bool _rememberDevice;
    private readonly SemaphoreSlim _refreshLock = new(1, 1);

    public async Task<AuthTokens?> GetTokensAsync()
    {
        if (_tokens != null)
        {
            return _tokens;
        }

        return await TryReadTokensAsync(LocalStorage, rememberDevice: true)
            ?? await TryReadTokensAsync(SessionStorage, rememberDevice: false);
    }

    public async Task SetTokensAsync(AuthTokens tokens, bool? rememberDevice = null)
    {
        _tokens = tokens;
        _rememberDevice = rememberDevice ?? _rememberDevice;

        try
        {
            var json = JsonSerializer.Serialize(tokens);
            if (_rememberDevice)
            {
                await jsRuntime.InvokeVoidAsync($"{LocalStorage}.setItem", StorageKey, json);
                await RemoveStorageItemAsync(SessionStorage);
            }
            else
            {
                await jsRuntime.InvokeVoidAsync($"{SessionStorage}.setItem", StorageKey, json);
                await RemoveStorageItemAsync(LocalStorage);
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
            await RemoveStorageItemAsync(LocalStorage);
            await RemoveStorageItemAsync(SessionStorage);
        }
        catch (InvalidOperationException)
        {
        }
        catch (JSException)
        {
        }
    }

    public async Task<bool> ClearTokensIfRefreshTokenMatchesAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return false;
        }

        var currentTokens = await GetTokensAsync();
        if (!string.Equals(currentTokens?.RefreshToken, refreshToken, StringComparison.Ordinal))
        {
            return false;
        }

        await ClearTokensAsync();
        return true;
    }

    public async Task<bool> RefreshTokensAsync(
        HttpClient http,
        string refreshUrl,
        string? attemptedAccessToken = null,
        Func<string, Task>? onAuthenticated = null,
        Action? onLoggedOut = null)
    {
        var requestedTokens = await GetTokensAsync();
        if (requestedTokens == null || string.IsNullOrWhiteSpace(requestedTokens.RefreshToken))
        {
            return false;
        }

        var requestedRefreshToken = requestedTokens.RefreshToken;

        await _refreshLock.WaitAsync();
        try
        {
            var currentTokens = await GetTokensAsync();
            if (currentTokens == null || string.IsNullOrWhiteSpace(currentTokens.RefreshToken))
            {
                return false;
            }

            if (TokenChanged(currentTokens, requestedRefreshToken, attemptedAccessToken))
            {
                if (!string.IsNullOrWhiteSpace(currentTokens.AccessToken) && onAuthenticated is not null)
                {
                    await onAuthenticated(currentTokens.AccessToken);
                }

                return true;
            }

            var response = await http.PostAsJsonAsync(refreshUrl, new
            {
                refreshToken = currentTokens.RefreshToken
            });

            if (!response.IsSuccessStatusCode)
            {
                if (await ClearTokensIfRefreshTokenMatchesAsync(currentTokens.RefreshToken))
                {
                    onLoggedOut?.Invoke();
                }

                return false;
            }

            var newTokens = await response.Content.ReadFromJsonAsync<AuthTokens>();
            if (newTokens == null ||
                string.IsNullOrWhiteSpace(newTokens.AccessToken) ||
                string.IsNullOrWhiteSpace(newTokens.RefreshToken))
            {
                if (await ClearTokensIfRefreshTokenMatchesAsync(currentTokens.RefreshToken))
                {
                    onLoggedOut?.Invoke();
                }

                return false;
            }

            await SetTokensAsync(newTokens);

            if (onAuthenticated is not null)
            {
                await onAuthenticated(newTokens.AccessToken);
            }

            return true;
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    private static bool TokenChanged(AuthTokens currentTokens, string requestedRefreshToken, string? attemptedAccessToken)
    {
        if (!string.Equals(currentTokens.RefreshToken, requestedRefreshToken, StringComparison.Ordinal))
        {
            return true;
        }

        return !string.IsNullOrWhiteSpace(attemptedAccessToken)
            && !string.Equals(currentTokens.AccessToken, attemptedAccessToken, StringComparison.Ordinal);
    }

    private async Task<AuthTokens?> TryReadTokensAsync(string storageName, bool rememberDevice)
    {
        try
        {
            var json = await jsRuntime.InvokeAsync<string?>($"{storageName}.getItem", StorageKey);
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            var tokens = JsonSerializer.Deserialize<AuthTokens>(json);
            if (tokens == null ||
                string.IsNullOrWhiteSpace(tokens.AccessToken) ||
                string.IsNullOrWhiteSpace(tokens.RefreshToken))
            {
                await RemoveStorageItemAsync(storageName);
                return null;
            }

            _tokens = tokens;
            _rememberDevice = rememberDevice;
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
            await RemoveStorageItemAsync(storageName);
            return null;
        }
    }

    private async Task RemoveStorageItemAsync(string storageName)
    {
        try
        {
            await jsRuntime.InvokeVoidAsync($"{storageName}.removeItem", StorageKey);
        }
        catch (InvalidOperationException)
        {
        }
        catch (JSException)
        {
        }
    }
}
