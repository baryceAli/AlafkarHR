using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Organization.Dtos;
using AlAfkarERP.Shared.Pages.Reuable2;
using AlAfkarERP.Shared.Services;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.Company.Services;

public class CompanyService : BaseApiService, ICompanyService
{
    private readonly string _path = "";
    private readonly ApiConfig _apiConfig;

    public CompanyService(HttpClient http, ITokenService tokenService, ApiConfig apiConfig) : base(http, tokenService, apiConfig)
    {
        _apiConfig = apiConfig;
        _path = $"api/{_apiConfig.Version}/organization/companies";
    }

    public async Task<ApiResult<CompanyDto>> CreateAsync(CompanyDto company)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}")
        {
            Content = JsonContent.Create(new
            {
                Company = company
            })
        };
        return await SendAsync<CompanyDto>(request, "createdCompany");
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid Id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_path}/{Id}");
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }

    public async Task<ApiResult<PagedResult<CompanyDto>>> GetAsync(int pageIndex, int pageSize)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}?PageIndex={pageIndex}&PageSize={pageSize}");
        return await SendAsync<PagedResult<CompanyDto>>(request, "companyList");
    }

    public async Task<ApiResult<PagedResult<CompanyDto>>> GetChildCompaniesAsync(int pageIndex, int pageSize, string? searchText = null)
    {
        var url = $"{_path}/child-companies?PageIndex={pageIndex}&PageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            url += $"&SearchText={Uri.EscapeDataString(searchText)}";
        }

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        return await SendAsync<PagedResult<CompanyDto>>(request, "companyList");
    }

    public async Task<ApiResult<CompanyLicenseSummaryDto>> GetCurrentLicenseAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/current/license");
        return await SendAsync<CompanyLicenseSummaryDto>(request, "license");
    }

    public async Task<ApiResult<CompanyDto>> GetByIdAsync(Guid Id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/{Id}");
        return await SendAsync<CompanyDto>(request, "company");
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(CompanyDto company)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}")
        {
            Content = JsonContent.Create(new
            {
                Company = company
            })
        };
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }

    public async Task<ApiResult<CompanyDto>> CreateChildAsync(CompanyDto company)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/child-companies")
        {
            Content = JsonContent.Create(new
            {
                Company = company
            })
        };
        return await SendAsync<CompanyDto>(request, "createdCompany");
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> UpdateChildAsync(CompanyDto company)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/child-companies")
        {
            Content = JsonContent.Create(new
            {
                Company = company
            })
        };
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> SetChildStatusAsync(Guid id, bool isActive)
    {
        var request = new HttpRequestMessage(HttpMethod.Patch, $"{_path}/child-companies/{id}/status")
        {
            Content = JsonContent.Create(new
            {
                IsActive = isActive
            })
        };
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> ResetChildAdminPasswordAsync(Guid id, string temporaryPassword)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/child-companies/{id}/admin/reset-password")
        {
            Content = JsonContent.Create(new
            {
                TemporaryPassword = temporaryPassword
            })
        };
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }
}
