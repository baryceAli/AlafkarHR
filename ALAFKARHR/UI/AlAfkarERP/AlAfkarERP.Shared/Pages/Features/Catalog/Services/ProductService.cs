
using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.Catalog.Dtos;
using System.Globalization;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.Catalog.Services;

public class ProductService : BaseApiService, IProductService
{
    //private readonly HttpClient _http;
    private readonly ApiConfig _apiConfig;
    //private readonly string _apiURL;
    public ProductService(HttpClient http, ITokenService tokenService, ApiConfig apiConfig) : base(http, tokenService, apiConfig)
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

    public async Task<ApiResult<PaginatedResult<ProductSkuDto>>> GetPublicStoreProductSkusAsync(int pageIndex, int pageSize)
        => await GetPublicStoreProductSkusAsync(new PublicStoreProductSkuRequest
        {
            PageIndex = pageIndex,
            PageSize = pageSize
        });

    public async Task<ApiResult<PaginatedResult<ProductSkuDto>>> GetPublicStoreProductSkusAsync(PublicStoreProductSkuRequest requestModel)
    {
        var request = new HttpRequestMessage(
            HttpMethod.Get,
            BuildPublicStoreProductSkuUrl(requestModel));

        return await SendAsync<PaginatedResult<ProductSkuDto>>(request, "productSkus");
    }

    public async Task<ApiResult<PublicStoreProductSkuFilterMetadataDto>> GetPublicStoreProductSkuFiltersAsync()
    {
        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"api/{_apiConfig.Version}/catalog/public/products/skus/filters");

        return await SendAsync<PublicStoreProductSkuFilterMetadataDto>(request, "metadata");
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

    public async Task<ApiResult<PaginatedResult<ProductDto>>> GetPricedByCompanyAsync(Guid companyId, Guid? customerId, int PageIndex, int PageSize, Guid? priceListId = null)
    {
        var query = new List<string>
        {
            $"PageIndex={PageIndex}",
            $"PageSize={PageSize}"
        };

        if (customerId.HasValue && customerId.Value != Guid.Empty)
        {
            query.Add($"customerId={customerId.Value}");
        }

        if (priceListId.HasValue && priceListId.Value != Guid.Empty)
        {
            query.Add($"priceListId={priceListId.Value}");
        }

        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"api/{_apiConfig.Version}/catalog/products/company/{companyId}/priced?{string.Join("&", query)}");

        return await SendAsync<PaginatedResult<ProductDto>>(request, "productList");
    }

    private string BuildPublicStoreProductSkuUrl(PublicStoreProductSkuRequest request)
    {
        var query = new List<string>
        {
            $"PageIndex={request.PageIndex}",
            $"PageSize={request.PageSize}",
            $"SortBy={Uri.EscapeDataString(request.SortBy ?? "newest")}",
            $"SortDescending={request.SortDescending.ToString().ToLowerInvariant()}"
        };

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query.Add($"SearchTerm={Uri.EscapeDataString(request.SearchTerm.Trim())}");
        }

        if (request.CategoryId.HasValue && request.CategoryId.Value != Guid.Empty)
        {
            query.Add($"CategoryId={request.CategoryId.Value}");
        }

        if (request.BrandId.HasValue && request.BrandId.Value != Guid.Empty)
        {
            query.Add($"BrandId={request.BrandId.Value}");
        }

        if (request.PackageId.HasValue && request.PackageId.Value != Guid.Empty)
        {
            query.Add($"PackageId={request.PackageId.Value}");
        }

        if (request.MinPrice.HasValue)
        {
            query.Add($"MinPrice={request.MinPrice.Value.ToString(CultureInfo.InvariantCulture)}");
        }

        if (request.MaxPrice.HasValue)
        {
            query.Add($"MaxPrice={request.MaxPrice.Value.ToString(CultureInfo.InvariantCulture)}");
        }

        if (request.CustomerId.HasValue && request.CustomerId.Value != Guid.Empty)
        {
            query.Add($"CustomerId={request.CustomerId.Value}");
        }

        return $"api/{_apiConfig.Version}/catalog/public/products/skus?{string.Join("&", query)}";
    }
}
