using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.Inventory.Dtos;
using SharedWithUI.Inventory.Enums;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.Inventories.Services;

public class WarehouseTransferService : BaseApiService, IWarehouseTransferService
{
    private readonly string _path;

    public WarehouseTransferService(HttpClient http, ITokenService tokenService, ApiConfig apiConfig) : base(http, tokenService, apiConfig)
    {
        _path = $"api/{apiConfig.Version}/inventory/warehouse-transfers";
    }

    public async Task<ApiResult<PaginatedResult<WarehouseTransferDto>>> GetAsync(Guid companyId, int pageIndex, int pageSize, string? searchText = null, TransferStatus? status = null)
    {
        var query = $"pageIndex={pageIndex}&pageSize={pageSize}&searchText={Uri.EscapeDataString(searchText ?? string.Empty)}";
        if (status.HasValue)
            query += $"&status={status.Value}";

        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/company/{companyId}?{query}");
        return await SendAsync<PaginatedResult<WarehouseTransferDto>>(request, "transferList");
    }

    public async Task<ApiResult<WarehouseTransferDto>> GetByIdAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/{id}");
        return await SendAsync<WarehouseTransferDto>(request, "transfer");
    }

    public async Task<ApiResult<CreateResponseDto>> CreateAsync(CreateWarehouseTransferDto transfer)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, _path)
        {
            Content = JsonContent.Create(transfer)
        };
        return await SendAsync<CreateResponseDto>(request, null);
    }

    public async Task<ApiResult<CreateResponseDto>> AddItemAsync(Guid transferId, WarehouseTransferItemInputDto item)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/{transferId}/items")
        {
            Content = JsonContent.Create(item)
        };
        return await SendAsync<CreateResponseDto>(request, null);
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> RemoveItemAsync(Guid transferId, Guid itemId)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_path}/{transferId}/items/{itemId}");
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> ShipAsync(Guid transferId)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/{transferId}/ship");
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> ReceiveAsync(Guid transferId, ReceiveWarehouseTransferItemDto item)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/{transferId}/receive")
        {
            Content = JsonContent.Create(item)
        };
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> CancelAsync(Guid transferId)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/{transferId}/cancel");
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }

    public async Task<ApiResult<List<TransferFefoBatchSuggestionDto>>> GetFefoBatchSuggestionsAsync(Guid companyId, Guid sourceWarehouseId, Guid productSkuId, decimal quantity)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/fefo-suggestions/company/{companyId}/warehouse/{sourceWarehouseId}/sku/{productSkuId}?quantity={quantity}");
        return await SendAsync<List<TransferFefoBatchSuggestionDto>>(request, "suggestions");
    }
}
