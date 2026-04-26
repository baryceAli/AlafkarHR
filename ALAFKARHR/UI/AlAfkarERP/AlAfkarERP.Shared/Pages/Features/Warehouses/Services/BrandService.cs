using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.Catalog.Dtos;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.Warehouses.Services;

public class BrandService : BaseApiService, IBrandService
{
    private readonly ApiConfig _apiConfig;

    public BrandService(HttpClient http, ApiConfig apiConfig) : base(http)
    {
        _apiConfig = apiConfig;
    }
    public async Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"api/{_apiConfig.Version}/catalog/Brands/{id}");
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }

    public async Task<ApiResult<PaginatedResult<BrandDto>>> GetAsync(int PageIndex, int PageSize)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/catalog/Brands?PageIndex={PageIndex}&PageSize={PageSize}");
        return await SendAsync<PaginatedResult<BrandDto>>(request, "brands");
    }

    public async Task<ApiResult<BrandDto>> GetByIdAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/catalog/Brands/{id}");
        return await SendAsync<BrandDto>(request, "brand");
    }

    public async Task<ApiResult<CreateResponseDto>> CreateAsync(BrandDto brand)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/{_apiConfig.Version}/catalog/Brands")
        {
            Content = JsonContent.Create(new
            {
                Brand = brand
            })
        };
        return await SendAsync<CreateResponseDto>(request, null);
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(BrandDto brand)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"api/{_apiConfig.Version}/catalog/Brands")
        {
            Content = JsonContent.Create(new
            {
                Brand = brand
            })
        };
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }

    public async Task<ApiResult<PaginatedResult<BrandDto>>> GetByCompanyIdAsync(Guid companyId, int pageIndex, int pageSize)
    {
        ///api/v1/catalog/brands/company/
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/catalog/Brands/company/{companyId}?PageIndex={pageIndex}&PageSize={pageSize}");
        return await SendAsync<PaginatedResult<BrandDto>>(request, "brandList");
    }
}
