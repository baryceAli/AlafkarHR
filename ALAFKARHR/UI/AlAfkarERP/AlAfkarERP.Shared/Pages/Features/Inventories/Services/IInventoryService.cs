using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Inventory.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.Inventories.Services;

public interface IInventoryService
{
    public Task<ApiResult<PaginatedResult<InventoryAggregateDto>>> GetAsync(int pageIndex, int pageSize, string? searchText, Guid? branchId = null);
    public Task<ApiResult<PaginatedResult<InventoryAggregateDto>>> GetAsync(Guid companyId, int pageIndex, int pageSize, string? searchText, Guid? branchId = null);
    public Task<ApiResult<InventoryAggregateDto>> GetByWarehouseAndSkuAsync(Guid warehouseId, Guid skuId);
    public Task<ApiResult<SkuAvailabilityDto>> GetSkuAvailabilityAsync(Guid companyId, Guid productSkuId, Guid? warehouseId = null, Guid? branchId = null);
    public Task<ApiResult<PaginatedResult<InventoryAggregateDto>>> GetByBatchAsync(Guid BatchId, int pageIndex, int pageSize, string? searchText);
    public Task<ApiResult<CreateResponseDto>> StockInAsync(CreateInventoryAggregateDto inventoryAggregateDto);
    public Task<ApiResult<CreateResponseDto>> StockOutAsync(CreateInventoryAggregateDto inventoryAggregateDto);
    public Task<ApiResult<CreateResponseDto>> StockAdjustmentAsync(CreateInventoryAggregateDto inventoryAggregateDto);
    public Task<ApiResult<CreateResponseDto>> ReserveAsync(CreateInventoryAggregateDto inventoryAggregateDto);
    public Task<ApiResult<CreateResponseDto>> ReleaseAsync(CreateInventoryAggregateDto inventoryAggregateDto);
    Task<ApiResult<List<WarehouseLocationDto>>> GetWarehouseLocationsAsync(Guid companyId);
    Task<ApiResult<CreateResponseDto>> SaveWarehouseLocationAsync(WarehouseLocationDto item);
    Task<ApiResult<string>> DeleteWarehouseLocationAsync(Guid id);
    Task<ApiResult<List<PutawayRuleDto>>> GetPutawayRulesAsync(Guid companyId);
    Task<ApiResult<CreateResponseDto>> SavePutawayRuleAsync(PutawayRuleDto item);
    Task<ApiResult<string>> DeletePutawayRuleAsync(Guid id);
    Task<ApiResult<List<QualityInspectionDto>>> GetQualityInspectionsAsync(Guid companyId);
    Task<ApiResult<CreateResponseDto>> SaveQualityInspectionAsync(QualityInspectionDto item);
    Task<ApiResult<string>> DeleteQualityInspectionAsync(Guid id);
    Task<ApiResult<List<LandedCostVoucherDto>>> GetLandedCostVouchersAsync(Guid companyId);
    Task<ApiResult<CreateResponseDto>> SaveLandedCostVoucherAsync(LandedCostVoucherDto item);
    Task<ApiResult<string>> PostLandedCostVoucherAsync(Guid id);
    Task<ApiResult<string>> DeleteLandedCostVoucherAsync(Guid id);
    Task<ApiResult<List<InventoryValuationLayerDto>>> GetValuationLayersAsync(Guid companyId);
    Task<ApiResult<List<ProjectedStockRowDto>>> GetProjectedStockAsync(Guid companyId);
    //public Task<ApiResult<CreateResponseDto>> AdjustmentAsync(CreateInventoryAggregateDto inventoryAggregateDto);
}
