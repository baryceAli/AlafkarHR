using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Pages.Features.Company.Dtos;
using AlAfkarERP.Shared.Pages.Reuable2;
using AlAfkarERP.Shared.Services;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.Company.Services;

public class AdministrationService : BaseApiService,IAdministrationService
{
    private readonly ApiConfig _apiConfig;
    private readonly string _path;
    public AdministrationService(HttpClient http, ApiConfig apiConfig) : base(http)
    {
        _apiConfig = apiConfig;
        //_path = $"{_apiConfig.BaseURL}/api/{_apiConfig.Version}/organization/administrations";
        _path = $"api/{_apiConfig.Version}/organization/administrations";
    }

    public async Task<ApiResult<AdministrationDto>> CreateAsync(AdministrationDto administration)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, _path)
        {
            Content = JsonContent.Create(new
            {
                Administration=administration
            })
        };
        return await SendAsync<AdministrationDto>(request, "createdAdministration");
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid Id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_path}/{Id}");
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }

    public async Task<ApiResult<PaginatedResult<AdministrationDto>>> GetAsync(int pageIndex, int pageSize)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}?PageIndex={pageIndex}&PageSize={pageSize}");
        return await SendAsync<PaginatedResult<AdministrationDto>>(request, "administrationList");
    }

    public async Task<ApiResult<PaginatedResult<AdministrationDto>>> GetByBranchAsync(Guid branchId, int pageIndex, int pageSize)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/BranchId/{branchId}?PageIndex={pageIndex}&PageSize={pageSize}");
        return await SendAsync<PaginatedResult<AdministrationDto>>(request, "administrationList");
    }

    public async Task<ApiResult<AdministrationDto>> GetByIdAsync(Guid Id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/{Id}");
        return await SendAsync<AdministrationDto>(request, "administration");
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(AdministrationDto administration)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}")
        {
            Content = JsonContent.Create(new
            {
                Administration=administration
            })
        };
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }
}
