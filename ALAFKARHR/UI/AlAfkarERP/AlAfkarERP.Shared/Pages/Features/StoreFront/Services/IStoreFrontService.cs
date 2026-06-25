using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Accounting.Dtos;
using SharedWithUI.StoreFront.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.StoreFront.Services;

public interface IStoreFrontService
{
    Task<ApiResult<List<StoreFrontTypeDto>>> GetTypesAsync(Guid companyId);
    Task<ApiResult<StoreFrontTypeDto>> SaveTypeAsync(StoreFrontTypeDto type);
    Task<ApiResult<bool>> DeleteTypeAsync(Guid id);
    Task<ApiResult<List<StoreFrontDto>>> GetStoresAsync(Guid companyId);
    Task<ApiResult<StoreFrontDto>> GetStoreAsync(Guid id);
    Task<ApiResult<StoreFrontDto>> SaveStoreAsync(StoreFrontDto store);
    Task<ApiResult<bool>> SetStoreStatusAsync(Guid id, bool isActive);
    Task<ApiResult<bool>> DeleteStoreAsync(Guid id);
    Task<ApiResult<List<StoreFrontSellableItemDto>>> GetItemsAsync(Guid storeFrontId);
    Task<ApiResult<bool>> SaveItemsAsync(Guid storeFrontId, List<StoreFrontSellableItemDto> items);
    Task<ApiResult<List<StoreFrontCatalogItemDto>>> GetCatalogAsync(Guid storeFrontId, Guid? customerId = null, string? searchText = null);
    Task<ApiResult<List<CashAccountDto>>> GetCashAccountsAsync(Guid storeFrontId);
    Task<ApiResult<Guid>> SaveCashAccountAsync(Guid storeFrontId, CashAccountDto cashAccount);
    Task<ApiResult<PosCashierSessionDto?>> GetOpenSessionAsync(Guid storeFrontId);
    Task<ApiResult<List<PosCashierSessionDto>>> GetOpenSessionsAsync(Guid storeFrontId);
    Task<ApiResult<PosCashierSessionDto>> OpenSessionAsync(OpenPosCashierSessionDto session);
    Task<ApiResult<PosCashierSessionDto>> CloseSessionAsync(Guid sessionId, ClosePosCashierSessionDto close);
    Task<ApiResult<PosCashierSessionSummaryDto>> GetSessionSummaryAsync(Guid storeFrontId, DateTime? fromDate = null, DateTime? toDate = null, bool ownOnly = false);
    Task<ApiResult<List<PosCashierSessionDto>>> GetSessionsAsync(Guid storeFrontId, DateTime? fromDate = null, DateTime? toDate = null, bool ownOnly = false);
}
