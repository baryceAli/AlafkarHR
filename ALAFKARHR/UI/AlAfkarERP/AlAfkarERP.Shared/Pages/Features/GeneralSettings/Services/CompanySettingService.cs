using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.GeneralSettings.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.GeneralSettings.Services;

public class CompanySettingService : BaseApiService, ICompanySettingService
{
    private readonly string _path;

    public CompanySettingService(HttpClient http, ApiConfig apiConfig) : base(http)
    {
        _path = $"api/{apiConfig.Version}/Settings";
    }

    public async Task<ApiResult<CompanySettingDto>> GetAsync(Guid companyId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/company/{companyId}/setting");
        return await SendAsync<CompanySettingDto>(request, "companySetting");
    }
}
