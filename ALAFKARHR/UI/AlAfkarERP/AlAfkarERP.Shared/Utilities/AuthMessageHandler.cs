using AlAfkarERP.Shared.Services.Auth;
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

        if (tokens != null)
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", tokens.AccessToken);
        }

        var response = await base.SendAsync(request, cancellationToken);

        // 🔥 AUTO REFRESH
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            var refreshed = await _authService.RefreshTokenAsync();

            if (!refreshed) return response;

            // retry request with new token
            var newTokens = await _tokenService.GetTokensAsync();

            //request.Headers.Authorization =
            //    new AuthenticationHeaderValue("Bearer", newTokens.AccessToken);
            //return await base.SendAsync(request, cancellationToken);

            var newRequest = new HttpRequestMessage(request.Method, request.RequestUri);

            if (request.Content != null)
            {
                var ms = new MemoryStream();
                await request.Content.CopyToAsync(ms);
                ms.Position = 0;
                newRequest.Content = new StreamContent(ms);

                foreach (var h in request.Content.Headers)
                    newRequest.Content.Headers.Add(h.Key, h.Value);
            }
            //foreach (var header in request.Headers)
            //{
            //    newRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
            //}

            newRequest.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", newTokens.AccessToken);

            return await base.SendAsync(newRequest, cancellationToken);

        }

        return response;
    }
}