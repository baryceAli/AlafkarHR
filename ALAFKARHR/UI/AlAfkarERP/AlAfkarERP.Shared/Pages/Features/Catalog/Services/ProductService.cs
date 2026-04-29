
using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.Catalog.Dtos;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.Catalog.Services;

public class ProductService : BaseApiService, IProductService
{
    //private readonly HttpClient _http;
    private readonly ApiConfig _apiConfig;
    //private readonly string _apiURL;
    public ProductService(HttpClient http,
        ApiConfig apiConfig) : base(http)
    {
        //_http = http;
        _apiConfig = apiConfig;
        //_apiURL = $"{_apiConfigOptions.BaseURL}/api{_apiConfigOptions.Version}";
    }

    

    public async Task<ApiResult<CreateResponseDto>> AddProductSkuAsync(ProductSkuDto productSku)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/{_apiConfig.Version}/catalog/products/skus")
        {
            Content = JsonContent.Create(new
            {
                ProductSku = productSku
            })
        };
        return await SendAsync<CreateResponseDto>(request, null);
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"api/{_apiConfig.Version}/catalog/products/{id}");
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }

    public async Task<ApiResult<PaginatedResult<ProductDto>>> GetAsync(Guid CategoryId, int PageIndex, int PageSize)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/catalog/products/GetByCategory/{CategoryId}?PageIndex={PageIndex}&PageSize={PageSize}");
        return await SendAsync<PaginatedResult<ProductDto>>(request, "productList");

    }

    public async Task<ApiResult<PaginatedResult<ProductDto>>> GetAsync(int PageIndex, int PageSize)
    {

        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/catalog/products?PageIndex={PageIndex}&PageSize={PageSize}");
        return await SendAsync<PaginatedResult<ProductDto>>(request, "productList");


    }

    public async Task<ApiResult<ProductDto>> GetByIdAsync(Guid productId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/catalog/products/{productId}");
        return await SendAsync<ProductDto>(request, "product");
    }

    public async Task<ApiResult<List<ProductDto>>> GetBySKUIds(List<Guid> skuIds)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/{_apiConfig.Version}/catalog/products/getBySkuIds")
        {
            Content=JsonContent.Create(new
            {
                productSkus = skuIds
            })
        };
        return await SendAsync<List<ProductDto>>(request, "productList");
    }

    public async Task<ApiResult<ProductSkuDto>> GetProductSkuByIdAsync(Guid productSkuId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/catalog/products/skus/{productSkuId}");
        return await SendAsync<ProductSkuDto>(request, "productSku");
    }

    public async Task<ApiResult<CreateResponseDto>> CreateAsync(ProductDto product)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/{_apiConfig.Version}/catalog/products")
        {
            Content = JsonContent.Create(new
            {
                Product = product
            })
        };
        return await SendAsync<CreateResponseDto>(request, null);
    }

    

    public async Task<ApiResult<UpdateDeleteResponseDto>> RemoveProductSkuAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"api/{_apiConfig.Version}/catalog/products/skus/{id}");
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }

    public async Task<ApiResult<PaginatedResult<ProductDto>>> SearchProductsAsync(string searchTerm, int page, int size)    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/catalog/products/search/{searchTerm}");
        return await SendAsync<PaginatedResult<ProductDto>>(request, "productList");
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(ProductDto product)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"api/{_apiConfig.Version}/catalog/products")
        {
            Content = JsonContent.Create(new
            {
                Product = product
            })
        };
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }

    

    public async Task<ApiResult<UpdateDeleteResponseDto>> UpdateProductSkuAsync(ProductSkuDto productSku)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"api/{_apiConfig.Version}/catalog/products/skus")
        {
            Content = JsonContent.Create(new
            {
                ProductSku = productSku
            })
        };
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }

    public async Task<ApiResult<PaginatedResult<ProductDto>>> GetByCompanyAsync(Guid companyId, int PageIndex, int PageSize)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, 
                    $"api/{_apiConfig.Version}/catalog/products/company/{companyId}?PageIndex={PageIndex}&PageSize={PageSize}");
        return await SendAsync<PaginatedResult<ProductDto>>(request, "productList");
    }
}
