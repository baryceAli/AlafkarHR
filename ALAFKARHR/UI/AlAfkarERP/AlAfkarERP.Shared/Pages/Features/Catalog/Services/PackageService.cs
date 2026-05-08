using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.Catalog.Dtos;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.Catalog.Services;

public class PackageService : BaseApiService, IPackageService
{
    private readonly ApiConfig _apiConfig;
    private readonly string _path;
    public PackageService(HttpClient http, ApiConfig apiConfig):base(http)
    {
        _apiConfig = apiConfig;
        _path = $"api/{_apiConfig.Version}/catalog/packages";
    }
    public async Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid id)
    {
        var request =new HttpRequestMessage(HttpMethod.Delete, $"{_path}/{id}");
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }

    public async Task<ApiResult<PaginatedResult<ProductPackageDto>>> GetAsync(int PageIndex, int PageSize)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}?PageIndex={PageIndex}&PageSize={PageSize}");
        return await SendAsync<PaginatedResult<ProductPackageDto>>(request, "productPackageList");
    }

    public async Task<ApiResult<PaginatedResult<ProductPackageDto>>> GetByCompanyAsync(Guid companyId,int PageIndex, int PageSize)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/company/{companyId}?PageIndex={PageIndex}&PageSize={PageSize}");
        return await SendAsync<PaginatedResult<ProductPackageDto>>(request, "productPackageList");
    }
    public async Task<ApiResult<ProductPackageDto>> GetByIdAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/{id}");
        return await SendAsync<ProductPackageDto>(request, "productPackage");
    }

    public async Task<ApiResult<CreateResponseDto>> CreateAsync(ProductPackageDto package)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}")
        {
            Content = JsonContent.Create(new
            {
                ProductPackage= package
            })
        };
        return await SendAsync<CreateResponseDto>(request, null);
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(ProductPackageDto package)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}")
        {
            Content = JsonContent.Create(new
            {
                ProductPackage = package
            })
        };
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }
}
