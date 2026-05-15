using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.Inventory.Dtos;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.Inventories.Services;

public class InventoryService : BaseApiService, IInventoryService
{
    private readonly ApiConfig _apiConfig;
    private string _path;
    public InventoryService(HttpClient http, ApiConfig apiConfig) : base(http)
    {
        _apiConfig = apiConfig;
        _path = $"api/{_apiConfig.Version}/inventory/inventories";
    }

    

    public async Task<ApiResult<PaginatedResult<InventoryAggregateDto>>> GetAsync(int pageIndex, int pageSize, string? searchText)
    {
        ///api/v1/inventory/inventories
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}");
        return await SendAsync<PaginatedResult<InventoryAggregateDto>>(request, "inventoryList");
    }
    public async Task<ApiResult<PaginatedResult<InventoryAggregateDto>>> GetAsync(Guid companyId, int pageIndex, int pageSize, string? searchText)
    {
        ///api/v1/inventory/inventories/company/{companyId}
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/company/{companyId}?pageIndex={pageIndex}&pageSize={pageSize}&searchText={searchText}");
        return await SendAsync<PaginatedResult<InventoryAggregateDto>>(request, "inventoryList");
    }

    public async Task<ApiResult<PaginatedResult<InventoryAggregateDto>>> GetByBatchAsync(Guid BatchId, int pageIndex, int pageSize, string? searchText)
    {
        ///api/v1/inventory/inventories/batch/{batchId}
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/batch/{BatchId}");
        return await SendAsync<PaginatedResult<InventoryAggregateDto>>(request, "inventoryList");
    }

    public async Task<ApiResult<InventoryAggregateDto>> GetByWarehouseAndSkuAsync(Guid warehouseId, Guid skuId)
    {
        ///api/v1/Inventory/inventories/warehouse/{warehouseId}/sku/{skuId}
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/warehouse/{warehouseId}/sku/{skuId}");
        return await SendAsync<InventoryAggregateDto>(request, "inventoryAggregate");
    }

    public async Task<ApiResult<CreateResponseDto>> ReserveAsync(CreateInventoryAggregateDto inventoryAggregateDto)
    {
        ///api/v1/inventory/inventories/StockReservation
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/StockReservation")
        {
            Content = JsonContent.Create(new
            {
                InventoryAggregate = inventoryAggregateDto
            })
        };
        return await SendAsync<CreateResponseDto>(request, null);
    }

    public async Task<ApiResult<CreateResponseDto>> ReleaseAsync(CreateInventoryAggregateDto inventoryAggregateDto)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/StockRelease")
        {
            Content = JsonContent.Create(new
            {
                InventoryAggregate = inventoryAggregateDto
            })
        };
        return await SendAsync<CreateResponseDto>(request, null);
    }

    public async Task<ApiResult<CreateResponseDto>> StockAdjustmentAsync(CreateInventoryAggregateDto inventoryAggregateDto)
    {
        ///api/v1/inventory/inventories/StockAdjustment
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/StockAdjustment")
        {
            Content = JsonContent.Create(new
            {
                InventoryAggregate = inventoryAggregateDto
            })
        };
        return await SendAsync<CreateResponseDto>(request, null);
    }

    public async Task<ApiResult<CreateResponseDto>> StockInAsync(CreateInventoryAggregateDto inventoryAggregateDto)
    {
        ///api/v1/inventory/inventories/StockIn
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/StockIn")
        {
            Content = JsonContent.Create(new
            {
                InventoryAggregate = inventoryAggregateDto
            })
        };
        return await SendAsync<CreateResponseDto>(request, null);
    }

    public async Task<ApiResult<CreateResponseDto>> StockOutAsync(CreateInventoryAggregateDto inventoryAggregateDto)
    {
        ///api/v1/inventory/inventories/StockIn
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/StockOut")
        {
            Content = JsonContent.Create(new
            {
                InventoryAggregate = inventoryAggregateDto
            })
        };
        return await SendAsync<CreateResponseDto>(request, null);
    }
}
