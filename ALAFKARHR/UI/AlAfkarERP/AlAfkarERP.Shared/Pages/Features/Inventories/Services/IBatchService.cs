using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Inventory.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.Inventories.Services;

public interface IBatchService
{
    public Task<ApiResult<PaginatedResult<BatchDto>>> GetAsync(Guid companyId,int PageIndex, int PageSize,string? searchText="");
    public Task<ApiResult<BatchDto>> GetByIdAsync(Guid Id);
    //public Task<ApiResult<List<BatchDto>>> GetByWarehouseId(Guid warehouseId);
    public Task<ApiResult<CreateResponseDto>> CreateAsync(CreateBatchDto createBatch);
    public Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(UpdateBatchDto updateBatch);
    public Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid Id);
}
