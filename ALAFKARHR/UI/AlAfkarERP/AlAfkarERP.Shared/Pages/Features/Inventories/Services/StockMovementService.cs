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

    public async Task<ApiResult<PaginatedResult<StockMovementDto>>> GetAsync(Guid companyId, int pageIndex, int pageSize, string? searchText = null, Guid? warehouseId = null, Guid? productSkuId = null, Guid? batchId = null)
    {
        var query = $"pageIndex={pageIndex}&pageSize={pageSize}&searchText={Uri.EscapeDataString(searchText ?? string.Empty)}";
        if (warehouseId.HasValue)
            query += $"&warehouseId={warehouseId.Value}";
        if (productSkuId.HasValue)
            query += $"&productSkuId={productSkuId.Value}";
        if (batchId.HasValue)
            query += $"&batchId={batchId.Value}";

        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/company/{companyId}?{query}");
        return await SendAsync<PaginatedResult<StockMovementDto>>(request, "movementList");
    }
}
