using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.StoreFront.Dtos;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.StoreFront.Services;

public class StoreFrontService : BaseApiService, IStoreFrontService
{
    private readonly string _path;

    public StoreFrontService(HttpClient http, ITokenService tokenService, ApiConfig apiConfig) : base(http, tokenService, apiConfig)
    {
        _path = $"api/{apiConfig.Version}/store-front";
    }

    public async Task<ApiResult<List<StoreFrontTypeDto>>> GetTypesAsync(Guid companyId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/types/company/{companyId}");
        return await SendAsync<List<StoreFrontTypeDto>>(request, "types");
    }

    public async Task<ApiResult<StoreFrontTypeDto>> SaveTypeAsync(StoreFrontTypeDto type)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/types")
        {
            Content = JsonContent.Create(new { Type = type })
        };
        return await SendAsync<StoreFrontTypeDto>(request, "type");
    }

    public async Task<ApiResult<List<StoreFrontDto>>> GetStoresAsync(Guid companyId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/stores/company/{companyId}");
        return await SendAsync<List<StoreFrontDto>>(request, "stores");
    }

    public async Task<ApiResult<StoreFrontDto>> GetStoreAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/stores/{id}");
        return await SendAsync<StoreFrontDto>(request, "store");
    }

    public async Task<ApiResult<StoreFrontDto>> SaveStoreAsync(StoreFrontDto store)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/stores")
        {
            Content = JsonContent.Create(new { Store = store })
        };
        return await SendAsync<StoreFrontDto>(request, "store");
    }

    public async Task<ApiResult<bool>> SetStoreStatusAsync(Guid id, bool isActive)
    {
        var request = new HttpRequestMessage(HttpMethod.Patch, $"{_path}/stores/{id}/status")
        {
            Content = JsonContent.Create(new { IsActive = isActive })
        };
        return await SendAsync<bool>(request, "isSuccess");
    }

    public async Task<ApiResult<List<StoreFrontSellableItemDto>>> GetItemsAsync(Guid storeFrontId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/stores/{storeFrontId}/items");
        return await SendAsync<List<StoreFrontSellableItemDto>>(request, "items");
    }

    public async Task<ApiResult<bool>> SaveItemsAsync(Guid storeFrontId, List<StoreFrontSellableItemDto> items)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/stores/{storeFrontId}/items")
        {
            Content = JsonContent.Create(new { Items = items })
        };
        return await SendAsync<bool>(request, "isSuccess");
    }

    public async Task<ApiResult<List<StoreFrontCatalogItemDto>>> GetCatalogAsync(Guid storeFrontId, Guid? customerId = null, string? searchText = null)
    {
        var query = new List<string>();
        if (customerId.HasValue)
            query.Add($"customerId={customerId.Value}");
        if (!string.IsNullOrWhiteSpace(searchText))
            query.Add($"searchText={Uri.EscapeDataString(searchText)}");

        var suffix = query.Count == 0 ? string.Empty : $"?{string.Join("&", query)}";
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/stores/{storeFrontId}/catalog{suffix}");
        return await SendAsync<List<StoreFrontCatalogItemDto>>(request, "items");
    }
}
