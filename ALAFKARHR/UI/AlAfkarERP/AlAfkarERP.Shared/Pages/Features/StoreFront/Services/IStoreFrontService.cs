using AlAfkarERP.Shared.Dtos;
using SharedWithUI.StoreFront.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.StoreFront.Services;

public interface IStoreFrontService
{
    Task<ApiResult<List<StoreFrontTypeDto>>> GetTypesAsync(Guid companyId);
    Task<ApiResult<StoreFrontTypeDto>> SaveTypeAsync(StoreFrontTypeDto type);
    Task<ApiResult<List<StoreFrontDto>>> GetStoresAsync(Guid companyId);
    Task<ApiResult<StoreFrontDto>> GetStoreAsync(Guid id);
    Task<ApiResult<StoreFrontDto>> SaveStoreAsync(StoreFrontDto store);
    Task<ApiResult<bool>> SetStoreStatusAsync(Guid id, bool isActive);
    Task<ApiResult<List<StoreFrontSellableItemDto>>> GetItemsAsync(Guid storeFrontId);
    Task<ApiResult<bool>> SaveItemsAsync(Guid storeFrontId, List<StoreFrontSellableItemDto> items);
    Task<ApiResult<List<StoreFrontCatalogItemDto>>> GetCatalogAsync(Guid storeFrontId, Guid? customerId = null, string? searchText = null);
}
