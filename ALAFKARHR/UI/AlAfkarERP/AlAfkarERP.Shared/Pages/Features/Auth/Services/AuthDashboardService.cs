using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.Auth.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.Auth.Services;

public class AuthDashboardService : BaseApiService, IAuthDashboardService
{
    private readonly ApiConfig _apiConfig;

    public AuthDashboardService(HttpClient http, ITokenService tokenService, ApiConfig apiConfig)
        : base(http, tokenService, apiConfig)
    {
        _apiConfig = apiConfig;
    }

    public Task<ApiResult<AuthDashboardDto>> GetDashboardAsync(Guid companyId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/auth/dashboard?companyId={companyId}");
        return SendAsync<AuthDashboardDto>(request, "dashboard");
    }
}
