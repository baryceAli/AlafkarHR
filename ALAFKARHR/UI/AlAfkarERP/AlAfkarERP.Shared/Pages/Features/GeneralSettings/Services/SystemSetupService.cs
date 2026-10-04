using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.GeneralSettings.Dtos;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.GeneralSettings.Services;

public sealed class SystemSetupService : BaseApiService, ISystemSetupService
{
    private readonly string path;

    public SystemSetupService(HttpClient http, ITokenService tokenService, ApiConfig apiConfig)
        : base(http, tokenService, apiConfig)
    {
        path = $"api/{apiConfig.Version}/GeneralSettings/setup/current";
    }

    public Task<ApiResult<SetupSummaryDto>> GetCurrentAsync()
        => SendAsync<SetupSummaryDto>(new HttpRequestMessage(HttpMethod.Get, path), "setup");

    public Task<ApiResult<SetupSummaryDto>> UpdateProgressAsync(UpdateSetupProgressDto progress)
        => SendAsync<SetupSummaryDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/progress")
        {
            Content = JsonContent.Create(new { Progress = progress })
        }, "setup");
}
