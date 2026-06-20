using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Inventory.Dtos;
using SharedWithUI.Inventory.Enums;

namespace AlAfkarERP.Shared.Pages.Features.Inventories.Services;

public interface IWarehouseTransferService
{
    Task<ApiResult<PaginatedResult<WarehouseTransferDto>>> GetAsync(Guid companyId, int pageIndex, int pageSize, string? searchText = null, TransferStatus? status = null);
    Task<ApiResult<WarehouseTransferDto>> GetByIdAsync(Guid id);
    Task<ApiResult<CreateResponseDto>> CreateAsync(CreateWarehouseTransferDto transfer);
    Task<ApiResult<CreateResponseDto>> AddItemAsync(Guid transferId, WarehouseTransferItemInputDto item);
    Task<ApiResult<UpdateDeleteResponseDto>> RemoveItemAsync(Guid transferId, Guid itemId);
    Task<ApiResult<UpdateDeleteResponseDto>> ShipAsync(Guid transferId);
    Task<ApiResult<UpdateDeleteResponseDto>> ReceiveAsync(Guid transferId, ReceiveWarehouseTransferItemDto item);
    Task<ApiResult<UpdateDeleteResponseDto>> CancelAsync(Guid transferId);
}
