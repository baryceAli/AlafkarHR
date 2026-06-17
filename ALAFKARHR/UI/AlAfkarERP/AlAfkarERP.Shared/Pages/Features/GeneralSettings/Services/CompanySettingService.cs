using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.GeneralSettings.Dtos;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.GeneralSettings.Services;

public class CompanySettingService : BaseApiService, ICompanySettingService
{
    private readonly string _path;

    public CompanySettingService(HttpClient http, ITokenService tokenService, ApiConfig apiConfig) : base(http, tokenService, apiConfig)
    {
        _path = $"api/{apiConfig.Version}/Settings";
    }

    public async Task<ApiResult<CompanySettingDto>> GetAsync(Guid companyId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/company/{companyId}/setting");
        return await SendAsync<CompanySettingDto>(request, "companySetting");
    }

    public async Task<ApiResult<CompanySettingDto>> UpdateAsync(Guid companyId, CompanySettingDto companySetting)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/company/{companyId}/setting")
        {
            Content = JsonContent.Create(new
            {
                CompanySetting = companySetting
            })
        };

        return await SendAsync<CompanySettingDto>(request, "companySetting");
    }
}
