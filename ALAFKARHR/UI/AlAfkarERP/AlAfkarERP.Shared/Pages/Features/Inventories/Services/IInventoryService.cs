using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Inventory.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.Inventories.Services;

public interface IInventoryService
{
    public Task<ApiResult<PaginatedResult<InventoryAggregateDto>>> GetAsync(int pageIndex, int pageSize, string? searchText);
    public Task<ApiResult<PaginatedResult<InventoryAggregateDto>>> GetAsync(Guid companyId, int pageIndex, int pageSize, string? searchText);
    public Task<ApiResult<InventoryAggregateDto>> GetByWarehouseAndSkuAsync(Guid warehouseId, Guid skuId);
    public Task<ApiResult<PaginatedResult<InventoryAggregateDto>>> GetByBatchAsync(Guid BatchId, int pageIndex, int pageSize, string? searchText);
    public Task<ApiResult<CreateResponseDto>> StockInAsync(CreateInventoryAggregateDto inventoryAggregateDto);
    public Task<ApiResult<CreateResponseDto>> StockOutAsync(CreateInventoryAggregateDto inventoryAggregateDto);
    public Task<ApiResult<CreateResponseDto>> StockAdjustmentAsync(CreateInventoryAggregateDto inventoryAggregateDto);
    public Task<ApiResult<CreateResponseDto>> ReserveAsync(CreateInventoryAggregateDto inventoryAggregateDto);
    public Task<ApiResult<CreateResponseDto>> ReleaseAsync(CreateInventoryAggregateDto inventoryAggregateDto);
    //public Task<ApiResult<CreateResponseDto>> AdjustmentAsync(CreateInventoryAggregateDto inventoryAggregateDto);
}
