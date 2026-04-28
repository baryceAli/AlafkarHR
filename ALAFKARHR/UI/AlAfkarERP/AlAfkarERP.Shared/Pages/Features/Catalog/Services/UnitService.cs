using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.Catalog.Dtos;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.Catalog.Services;

public class UnitService : BaseApiService, IUnitService
{
    private readonly ApiConfig _apiConfig;

    public UnitService(HttpClient http,ApiConfig apiConfig):base(http)
    {
        _apiConfig = apiConfig;
    }
    public async Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"api/{_apiConfig.Version}/catalog/units/{id}");
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }

    //public async Task<ApiResult<PaginatedResult<UnitDto>>> GetAsync(int PageIndex, int PageSize)
    //{
    //    var request = new HttpRequestMessage(HttpMethod.Get, $"api{_apiConfig.Version}/catalog/units?PageIndex={PageIndex}&PageSize={PageSize}");
    //    return await SendAsync<PaginatedResult<UnitDto>>(request, "unitList");
    //}

    public async Task<ApiResult<UnitDto>> GetByIdAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/catalog/units/{id}");
        return await SendAsync<UnitDto>(request, "unit");
    }

    public async Task<ApiResult<CreateResponseDto>> CreateAsync(UnitDto unit)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/{_apiConfig.Version}/catalog/units")
        {
            Content = JsonContent.Create(new
            {
                Unit = unit
            })
        };
        return await SendAsync<CreateResponseDto>(request, null);
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(UnitDto unit)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"api/{_apiConfig.Version}/catalog/units")
        {
            Content = JsonContent.Create(new
            {
                Unit = unit
            })
        };
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }

    public async Task<ApiResult<PaginatedResult<UnitDto>>> GetAsync(int PageIndex, int PageSize, string? searchText = "")
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/catalog/units?PageIndex={PageIndex}&PageSize={PageSize}&searchText={searchText}");
        return await SendAsync<PaginatedResult<UnitDto>>(request, "unitList");
    }

    public async Task<ApiResult<PaginatedResult<UnitDto>>> GetByCompanyAsync(Guid companyId, int PageIndex, int PageSize, string? searchText = "")
    {
        var request = new HttpRequestMessage(HttpMethod.Get, 
            $"api/{_apiConfig.Version}/catalog/units/company/{companyId}?PageIndex={PageIndex}&PageSize={PageSize}&searchText={searchText}");
        return await SendAsync<PaginatedResult<UnitDto>>(request, "unitList");

    }
}
