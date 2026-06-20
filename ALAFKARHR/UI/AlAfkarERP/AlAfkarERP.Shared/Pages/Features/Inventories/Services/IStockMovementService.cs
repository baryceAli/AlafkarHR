using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Inventory.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.Inventories.Services;

public interface IStockMovementService
{
    Task<ApiResult<PaginatedResult<StockMovementDto>>> GetAsync(
        Guid companyId,
        int pageIndex,
        int pageSize,
        string? searchText = null,
        Guid? warehouseId = null,
        Guid? productSkuId = null,
        Guid? batchId = null,
        string? sourceDocumentType = null,
        string? referenceNumber = null,
        DateTime? fromDate = null,
        DateTime? toDate = null);
}
