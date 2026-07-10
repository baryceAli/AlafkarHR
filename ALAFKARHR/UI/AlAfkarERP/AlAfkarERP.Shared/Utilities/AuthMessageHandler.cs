using AlAfkarERP.Shared.Pages.Features.Auth.Services;
using System.Net;
using System.Net.Http.Headers;

namespace AlAfkarERP.Shared.Utilities;

public class AuthMessageHandler : DelegatingHandler
{
    private readonly ITokenService _tokenService;
    private readonly IAuthService _authService;

    public AuthMessageHandler(ITokenService tokenService, IAuthService authService)
    {
        _tokenService = tokenService;
        _authService = authService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var tokens = await _tokenService.GetTokensAsync();

        var attemptedAccessToken = tokens?.AccessToken;
        if (tokens != null)
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", tokens.AccessToken);
        }

        var retryRequest = await CloneHttpRequestMessageAsync(request, cancellationToken);
        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode != HttpStatusCode.Unauthorized)
        {
            retryRequest.Dispose();
            return response;
        }

        var refreshed = await _authService.RefreshTokenAsync();
        if (!refreshed)
        {
            var currentTokens = await _tokenService.GetTokensAsync();
            if (currentTokens == null ||
                string.IsNullOrWhiteSpace(currentTokens.AccessToken) ||
                string.Equals(currentTokens.AccessToken, attemptedAccessToken, StringComparison.Ordinal))
            {
                return response;
            }
        }

        var newTokens = await _tokenService.GetTokensAsync();
        if (newTokens == null || string.IsNullOrWhiteSpace(newTokens.AccessToken))
            return response;

        response.Dispose();
        retryRequest.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", newTokens.AccessToken);

        return await base.SendAsync(retryRequest, cancellationToken);
    }

    private static async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var clone = new HttpRequestMessage(request.Method, request.RequestUri)
        {
            Version = request.Version,
            VersionPolicy = request.VersionPolicy
        };

        foreach (var header in request.Headers)
        {
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        foreach (var option in request.Options)
        {
            clone.Options.TryAdd(option.Key, option.Value);
        }

        if (request.Content != null)
        {
            var ms = new MemoryStream();
            await request.Content.CopyToAsync(ms, cancellationToken);
            ms.Position = 0;
            clone.Content = new StreamContent(ms);

            foreach (var header in request.Content.Headers)
            {
                clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        return clone;
    }
}
