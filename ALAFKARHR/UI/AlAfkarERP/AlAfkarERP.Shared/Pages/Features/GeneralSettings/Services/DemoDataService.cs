using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.GeneralSettings.Dtos;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.GeneralSettings.Services;

public sealed class DemoDataService : BaseApiService, IDemoDataService
{
    private readonly string _path;

    public DemoDataService(HttpClient http, ITokenService tokenService, ApiConfig apiConfig)
        : base(http, tokenService, apiConfig)
    {
        _path = $"api/{apiConfig.Version}/demo-data";
    }

    public Task<ApiResult<List<DemoDataSummaryDto>>> ListAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, _path);
        return SendAsync<List<DemoDataSummaryDto>>(request, "demos");
    }

    public Task<ApiResult<DemoDataStatusDto>> GetStatusAsync(string companyCode)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/{Uri.EscapeDataString(companyCode)}/status");
        return SendAsync<DemoDataStatusDto>(request, "status");
    }

    public Task<ApiResult<DemoDataOperationResultDto>> CreateAsync(DemoDataCreateRequestDto createRequest)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/create")
        {
            Content = JsonContent.Create(createRequest)
        };
        return SendAsync<DemoDataOperationResultDto>(request, "result");
    }

    public Task<ApiResult<DemoDataOperationResultDto>> ResetAsync(string companyCode)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/{Uri.EscapeDataString(companyCode)}/reset")
        {
            Content = JsonContent.Create(new DemoDataConfirmationRequestDto { CompanyCode = companyCode })
        };
        return SendAsync<DemoDataOperationResultDto>(request, "result");
    }

    public Task<ApiResult<DemoDataOperationResultDto>> DeleteAsync(string companyCode)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_path}/{Uri.EscapeDataString(companyCode)}")
        {
            Content = JsonContent.Create(new DemoDataConfirmationRequestDto { CompanyCode = companyCode })
        };
        return SendAsync<DemoDataOperationResultDto>(request, "result");
    }

    public Task<ApiResult<DemoDataOperationResultDto>> ResetAdminPasswordAsync(string companyCode)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/{Uri.EscapeDataString(companyCode)}/admin-password/reset")
        {
            Content = JsonContent.Create(new DemoDataConfirmationRequestDto { CompanyCode = companyCode })
        };
        return SendAsync<DemoDataOperationResultDto>(request, "result");
    }
}
