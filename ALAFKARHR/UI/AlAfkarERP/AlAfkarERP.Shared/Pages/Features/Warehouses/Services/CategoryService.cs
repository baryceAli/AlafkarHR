using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.Catalog.Dtos;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.Warehouses.Services;

public class CategoryService : BaseApiService, ICategoryService
{
    private readonly ApiConfig _apiConfig;

    public CategoryService(HttpClient http,ApiConfig apiConfig) : base(http)
    {
        _apiConfig = apiConfig;
    }

    public async Task<ApiResult<CreateResponseDto>> CreateAsync(CategoryDto category)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/{_apiConfig.Version}/catalog/categories")
        {
            Content = JsonContent.Create (new
            {
                Category=category
            })
        };

        return await SendAsync<CreateResponseDto>(request,null);
    }

    public async Task<ApiResult<CategoryDto>> GetByIdAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/catalog/categories/{id}");

        return await SendAsync<CategoryDto>(request, "category");
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(CategoryDto category)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"api/{_apiConfig.Version}/catalog/categories")
        {
            Content = JsonContent.Create(new
            {
                Category = category
            })
        };

        return await SendAsync<UpdateDeleteResponseDto>(request,null);
    }

    //public async Task<ApiResult<PaginatedResult<CategoryDto>>> GetAsync(int PageIndex, int PageSize)
    //{
    //    var request = new HttpRequestMessage(HttpMethod.Get, $"api/ {_apiConfig.Version}/catalog/categories?PageIndex={PageIndex}&PageSize={PageSize}");
    //    return await SendAsync<PaginatedResult<CategoryDto>>(request, "categoryList");
    //}

    public async Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"api/{_apiConfig.Version}/catalog/categories/{id}");
        return await SendAsync<UpdateDeleteResponseDto>(request,null);
    }

    public async Task<ApiResult<PaginatedResult<CategoryDto>>> GetByCompanyIdAsync(Guid companyId, int PageIndex, int PageSize)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/catalog/categories/company/{companyId}?PageIndex={PageIndex}&PageSize={PageSize}");
        return await SendAsync<PaginatedResult<CategoryDto>>(request, "categoryList");
    }
}