
using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Inventory.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.Inventories.Services;

public interface IWarehouseService
{
    public Task<ApiResult<PaginatedResult<WarehouseDto>>> GetAsync(Guid companyId,int PageIndex, int PageSize, string searchTex="");
    public Task<ApiResult<WarehouseDto>> GetByIdAsync(Guid id);
    public Task<ApiResult<CreateResponseDto>> CreateAsync(WarehouseDto warehouse);
    public Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid id);
    public Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(WarehouseDto warehouse);
}
