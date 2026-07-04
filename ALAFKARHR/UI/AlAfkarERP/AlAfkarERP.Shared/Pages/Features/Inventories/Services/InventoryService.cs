using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.Inventory.Dtos;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.Inventories.Services;

public class InventoryService : BaseApiService, IInventoryService
{
    private readonly ApiConfig _apiConfig;
    private string _path;
    public InventoryService(HttpClient http, ITokenService tokenService, ApiConfig apiConfig) : base(http, tokenService, apiConfig)
    {
        _apiConfig = apiConfig;
        _path = $"api/{_apiConfig.Version}/inventory/inventories";
    }

    

    public async Task<ApiResult<PaginatedResult<InventoryAggregateDto>>> GetAsync(int pageIndex, int pageSize, string? searchText, Guid? branchId = null)
    {
        ///api/v1/inventory/inventories
        var url = $"{_path}?companyId={_apiConfig.CompanyId}&pageIndex={pageIndex}&pageSize={pageSize}&searchText={Uri.EscapeDataString(searchText ?? string.Empty)}";
        if (branchId.HasValue)
            url += $"&branchId={branchId.Value}";

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        return await SendAsync<PaginatedResult<InventoryAggregateDto>>(request, "inventoryList");
    }
    public async Task<ApiResult<PaginatedResult<InventoryAggregateDto>>> GetAsync(Guid companyId, int pageIndex, int pageSize, string? searchText, Guid? branchId = null)
    {
        ///api/v1/inventory/inventories/company/{companyId}
        var url = $"{_path}/company/{companyId}?pageIndex={pageIndex}&pageSize={pageSize}&searchText={Uri.EscapeDataString(searchText ?? string.Empty)}";
        if (branchId.HasValue)
            url += $"&branchId={branchId.Value}";

        var request = new HttpRequestMessage(HttpMethod.Get, url);
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

    public async Task<ApiResult<SkuAvailabilityDto>> GetSkuAvailabilityAsync(Guid companyId, Guid productSkuId, Guid? warehouseId = null, Guid? branchId = null)
    {
        var url = $"api/{_apiConfig.Version}/inventory/availability/company/{companyId}/sku/{productSkuId}";
        var query = new List<string>();
        if (warehouseId.HasValue)
            query.Add($"warehouseId={warehouseId.Value}");
        if (branchId.HasValue)
            query.Add($"branchId={branchId.Value}");
        if (query.Any())
            url += $"?{string.Join("&", query)}";

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        return await SendAsync<SkuAvailabilityDto>(request, "availability");
    }

    public async Task<ApiResult<CreateResponseDto>> ReserveAsync(CreateInventoryAggregateDto inventoryAggregateDto)
    {
        EnsureAudit(inventoryAggregateDto, "InventoryReservation");
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
        EnsureAudit(inventoryAggregateDto, "InventoryRelease");
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
        EnsureAudit(inventoryAggregateDto, "InventoryAdjustment");
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
        EnsureAudit(inventoryAggregateDto, "InventoryStockIn");
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
        EnsureAudit(inventoryAggregateDto, "InventoryStockOut");
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

    public async Task<ApiResult<List<WarehouseLocationDto>>> GetWarehouseLocationsAsync(Guid companyId) =>
        await GetControlListAsync<WarehouseLocationDto>("warehouse-locations", companyId);

    public async Task<ApiResult<CreateResponseDto>> SaveWarehouseLocationAsync(WarehouseLocationDto item) =>
        await SaveControlAsync("warehouse-locations", item);

    public async Task<ApiResult<string>> DeleteWarehouseLocationAsync(Guid id) =>
        await DeleteControlAsync("warehouse-locations", id);

    public async Task<ApiResult<List<PutawayRuleDto>>> GetPutawayRulesAsync(Guid companyId) =>
        await GetControlListAsync<PutawayRuleDto>("putaway-rules", companyId);

    public async Task<ApiResult<CreateResponseDto>> SavePutawayRuleAsync(PutawayRuleDto item) =>
        await SaveControlAsync("putaway-rules", item);

    public async Task<ApiResult<string>> DeletePutawayRuleAsync(Guid id) =>
        await DeleteControlAsync("putaway-rules", id);

    public async Task<ApiResult<List<QualityInspectionDto>>> GetQualityInspectionsAsync(Guid companyId) =>
        await GetControlListAsync<QualityInspectionDto>("quality-inspections", companyId);

    public async Task<ApiResult<CreateResponseDto>> SaveQualityInspectionAsync(QualityInspectionDto item) =>
        await SaveControlAsync("quality-inspections", item);

    public async Task<ApiResult<string>> DeleteQualityInspectionAsync(Guid id) =>
        await DeleteControlAsync("quality-inspections", id);

    public async Task<ApiResult<List<LandedCostVoucherDto>>> GetLandedCostVouchersAsync(Guid companyId) =>
        await GetControlListAsync<LandedCostVoucherDto>("landed-cost-vouchers", companyId);

    public async Task<ApiResult<CreateResponseDto>> SaveLandedCostVoucherAsync(LandedCostVoucherDto item) =>
        await SaveControlAsync("landed-cost-vouchers", item);

    public async Task<ApiResult<string>> PostLandedCostVoucherAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/{_apiConfig.Version}/inventory/controls/landed-cost-vouchers/{id}/post");
        return await SendAsync<string>(request, null);
    }

    public async Task<ApiResult<string>> DeleteLandedCostVoucherAsync(Guid id) =>
        await DeleteControlAsync("landed-cost-vouchers", id);

    public async Task<ApiResult<List<InventoryValuationLayerDto>>> GetValuationLayersAsync(Guid companyId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/inventory/controls/valuation-layers/company/{companyId}");
        return await SendAsync<List<InventoryValuationLayerDto>>(request, "items");
    }

    public async Task<ApiResult<List<ProjectedStockRowDto>>> GetProjectedStockAsync(Guid companyId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/inventory/controls/projected-stock/company/{companyId}");
        return await SendAsync<List<ProjectedStockRowDto>>(request, "rows");
    }

    public async Task<ApiResult<List<InventoryLocationBalanceDto>>> GetLocationBalancesAsync(Guid companyId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/inventory/controls/location-balances/company/{companyId}");
        return await SendAsync<List<InventoryLocationBalanceDto>>(request, "rows");
    }

    public async Task<ApiResult<List<CycleCountDto>>> GetCycleCountsAsync(Guid companyId) =>
        await GetControlListAsync<CycleCountDto>("cycle-counts", companyId);

    public async Task<ApiResult<CreateResponseDto>> SaveCycleCountAsync(CycleCountDto item) =>
        await SaveControlAsync("cycle-counts", item);

    public async Task<ApiResult<string>> PostCycleCountAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/{_apiConfig.Version}/inventory/controls/cycle-counts/{id}/post");
        return await SendAsync<string>(request, null);
    }

    public async Task<ApiResult<string>> DeleteCycleCountAsync(Guid id) =>
        await DeleteControlAsync("cycle-counts", id);

    public async Task<ApiResult<LocationAvailabilityDto>> GetSkuLocationAvailabilityAsync(Guid companyId, Guid productSkuId, Guid? warehouseId = null, Guid? warehouseLocationId = null, Guid? batchId = null, Guid? branchId = null)
    {
        var url = $"api/{_apiConfig.Version}/inventory/location-availability/company/{companyId}/sku/{productSkuId}";
        var query = new List<string>();
        if (warehouseId.HasValue)
            query.Add($"warehouseId={warehouseId.Value}");
        if (warehouseLocationId.HasValue)
            query.Add($"warehouseLocationId={warehouseLocationId.Value}");
        if (batchId.HasValue)
            query.Add($"batchId={batchId.Value}");
        if (branchId.HasValue)
            query.Add($"branchId={branchId.Value}");
        if (query.Any())
            url += $"?{string.Join("&", query)}";

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        return await SendAsync<LocationAvailabilityDto>(request, "availability");
    }

    public async Task<ApiResult<PutawaySuggestionDto>> GetPutawaySuggestionAsync(Guid companyId, Guid warehouseId, Guid productId, Guid productSkuId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/inventory/controls/putaway-suggestion/company/{companyId}/warehouse/{warehouseId}/product/{productId}/sku/{productSkuId}");
        return await SendAsync<PutawaySuggestionDto>(request, "suggestion");
    }

    public async Task<ApiResult<BarcodeScanResultDto>> ResolveBarcodeAsync(BarcodeScanRequestDto scanRequest)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/{_apiConfig.Version}/inventory/barcode/resolve")
        {
            Content = JsonContent.Create(scanRequest)
        };
        return await SendAsync<BarcodeScanResultDto>(request, "result");
    }

    public async Task<ApiResult<CreateResponseDto>> CreateBarcodeSessionAsync(BarcodeOperationSessionDto session)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/{_apiConfig.Version}/inventory/barcode/sessions")
        {
            Content = JsonContent.Create(session)
        };
        return await SendAsync<CreateResponseDto>(request, null);
    }

    public async Task<ApiResult<BarcodeOperationSessionDto>> ScanBarcodeSessionAsync(Guid sessionId, BarcodeScanRequestDto scanRequest)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/{_apiConfig.Version}/inventory/barcode/sessions/{sessionId}/scan")
        {
            Content = JsonContent.Create(scanRequest)
        };
        return await SendAsync<BarcodeOperationSessionDto>(request, "session");
    }

    public async Task<ApiResult<BarcodeApplyResultDto>> ApplyBarcodeSessionAsync(Guid sessionId, bool confirmWarnings)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/{_apiConfig.Version}/inventory/barcode/sessions/{sessionId}/apply")
        {
            Content = JsonContent.Create(new ApplyBarcodeSessionDto { SessionId = sessionId, ConfirmWarnings = confirmWarnings })
        };
        return await SendAsync<BarcodeApplyResultDto>(request, "result");
    }

    public async Task<ApiResult<List<BarcodeOperationSessionDto>>> GetBarcodeSessionsAsync(Guid companyId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/inventory/barcode/sessions/company/{companyId}");
        return await SendAsync<List<BarcodeOperationSessionDto>>(request, "sessions");
    }

    private async Task<ApiResult<List<T>>> GetControlListAsync<T>(string route, Guid companyId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/inventory/controls/{route}/company/{companyId}");
        return await SendAsync<List<T>>(request, "items");
    }

    private async Task<ApiResult<CreateResponseDto>> SaveControlAsync<T>(string route, T item)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/{_apiConfig.Version}/inventory/controls/{route}")
        {
            Content = JsonContent.Create(item)
        };
        return await SendAsync<CreateResponseDto>(request, null);
    }

    private async Task<ApiResult<string>> DeleteControlAsync(string route, Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"api/{_apiConfig.Version}/inventory/controls/{route}/{id}");
        return await SendAsync<string>(request, null);
    }

    private static void EnsureAudit(CreateInventoryAggregateDto dto, string sourceDocumentType)
    {
        dto.SourceDocumentType = string.IsNullOrWhiteSpace(dto.SourceDocumentType)
            ? sourceDocumentType
            : dto.SourceDocumentType;

        dto.ReferenceNumber = string.IsNullOrWhiteSpace(dto.ReferenceNumber)
            ? $"{sourceDocumentType}-{DateTime.UtcNow:yyyyMMddHHmmssfff}"
            : dto.ReferenceNumber;
    }
}
