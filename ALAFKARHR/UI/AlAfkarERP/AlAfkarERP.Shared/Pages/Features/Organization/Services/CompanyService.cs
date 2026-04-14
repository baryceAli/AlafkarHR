using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Pages.Features.Company.Dtos;
using AlAfkarERP.Shared.Pages.Reuable;
using AlAfkarERP.Shared.Services;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.Company.Services;

public class CompanyService : BaseApiService, ICompanyService
{
    private readonly string _path = "";
    private readonly ApiConfig _apiConfig;

    public CompanyService(HttpClient http, ApiConfig apiConfig) : base(http)
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

    public async Task<ApiResult<CompanyDto>> GetByIdAsync(Guid Id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/{Id}");
        return await SendAsync<CompanyDto>(request, "company");
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(CompanyDto company)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}");
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }
}
