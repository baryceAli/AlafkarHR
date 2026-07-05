using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.Inventory.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.Inventories.Services;

public class StockMovementService : BaseApiService, IStockMovementService
{
    private readonly string _path;

    public StockMovementService(HttpClient http, ITokenService tokenService, ApiConfig apiConfig) : base(http, tokenService, apiConfig)
    {
        _path = $"api/{apiConfig.Version}/inventory/stock-movements";
    }

    public async Task<ApiResult<PaginatedResult<StockMovementDto>>> GetAsync(
        Guid companyId,
        int pageIndex,
        int pageSize,
        string? searchText = null,
        Guid? warehouseId = null,
        Guid? productSkuId = null,
        Guid? batchId = null,
        string? sourceDocumentType = null,
        Guid? sourceDocumentId = null,
        string? referenceNumber = null,
        Guid? parentProductSkuId = null,
        string? serialNumber = null,
        bool? expiredOnly = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        Guid? branchId = null)
    {
        var query = $"pageIndex={pageIndex}&pageSize={pageSize}&searchText={Uri.EscapeDataString(searchText ?? string.Empty)}";
        if (branchId.HasValue)
            query += $"&branchId={branchId.Value}";
        if (warehouseId.HasValue)
            query += $"&warehouseId={warehouseId.Value}";
        if (productSkuId.HasValue)
            query += $"&productSkuId={productSkuId.Value}";
        if (batchId.HasValue)
            query += $"&batchId={batchId.Value}";
        if (!string.IsNullOrWhiteSpace(sourceDocumentType))
            query += $"&sourceDocumentType={Uri.EscapeDataString(sourceDocumentType)}";
        if (sourceDocumentId.HasValue)
            query += $"&sourceDocumentId={sourceDocumentId.Value}";
        if (!string.IsNullOrWhiteSpace(referenceNumber))
            query += $"&referenceNumber={Uri.EscapeDataString(referenceNumber)}";
        if (parentProductSkuId.HasValue)
            query += $"&parentProductSkuId={parentProductSkuId.Value}";
        if (!string.IsNullOrWhiteSpace(serialNumber))
            query += $"&serialNumber={Uri.EscapeDataString(serialNumber)}";
        if (expiredOnly.HasValue)
            query += $"&expiredOnly={expiredOnly.Value}";
        if (fromDate.HasValue)
            query += $"&fromDate={Uri.EscapeDataString(fromDate.Value.ToString("O"))}";
        if (toDate.HasValue)
            query += $"&toDate={Uri.EscapeDataString(toDate.Value.ToString("O"))}";

        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/company/{companyId}?{query}");
        return await SendAsync<PaginatedResult<StockMovementDto>>(request, "movementList");
    }
}
