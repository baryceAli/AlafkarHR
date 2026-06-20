using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.Organization.Dtos;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.Company.Services;

public class ParentCompanyService : BaseApiService, IParentCompanyService
{
    private readonly string _path;

    public ParentCompanyService(HttpClient http, ITokenService tokenService, ApiConfig apiConfig) : base(http, tokenService, apiConfig)
    {
        _path = $"api/{apiConfig.Version}/organization/parent-companies";
    }

    public async Task<ApiResult<PaginatedResult<ParentCompanyDto>>> GetAsync(int pageIndex, int pageSize, string? searchText = null)
    {
        var url = $"{_path}?PageIndex={pageIndex}&PageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(searchText))
            url += $"&SearchText={Uri.EscapeDataString(searchText)}";

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        return await SendAsync<PaginatedResult<ParentCompanyDto>>(request, "companyList");
    }

    public async Task<ApiResult<ParentCompanyDto>> GetByIdAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/{id}");
        return await SendAsync<ParentCompanyDto>(request, "company");
    }

    public async Task<ApiResult<ParentCompanyDto>> CreateAsync(ParentCompanyDto company)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, _path)
        {
            Content = JsonContent.Create(new { Company = company })
        };
        return await SendAsync<ParentCompanyDto>(request, "createdCompany");
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(ParentCompanyDto company)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, _path)
        {
            Content = JsonContent.Create(new { Company = company })
        };
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> UpdateLicenseAsync(Guid id, CompanyLicenseDto license)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/{id}/license")
        {
            Content = JsonContent.Create(new { License = license })
        };
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> SetStatusAsync(Guid id, bool isActive)
    {
        var request = new HttpRequestMessage(HttpMethod.Patch, $"{_path}/{id}/status")
        {
            Content = JsonContent.Create(new { IsActive = isActive })
        };
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> ResetAdminPasswordAsync(Guid id, string temporaryPassword)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/{id}/admin/reset-password")
        {
            Content = JsonContent.Create(new { TemporaryPassword = temporaryPassword })
        };
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_path}/{id}");
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }
}
