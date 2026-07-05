using AlAfkarERP.Shared.Pages.Features.Auth.Services;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace AlAfkarERP.Shared.Utilities;

public static class PermissionAuthStateRefresher
{
    public static async Task<AuthenticationState> RefreshOnceIfMissingPermissionAsync(
        AuthenticationStateProvider authenticationStateProvider,
        IAuthService authService,
        string permission)
    {
        var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
        if (!ShouldTryRefresh(authState.User, permission))
        {
            return authState;
        }

        var refreshed = await authService.RefreshTokenAsync();
        return refreshed
            ? await authenticationStateProvider.GetAuthenticationStateAsync()
            : authState;
    }

    private static bool ShouldTryRefresh(ClaimsPrincipal user, string permission)
    {
        return user.Identity?.IsAuthenticated == true
            && !string.IsNullOrWhiteSpace(permission)
            && !user.HasClaim("Permission", permission);
    }
}
