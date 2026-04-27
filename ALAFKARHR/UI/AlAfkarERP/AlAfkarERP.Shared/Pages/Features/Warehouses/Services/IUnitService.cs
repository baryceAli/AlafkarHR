using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Catalog.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.Warehouses.Services;

public interface IUnitService
{
    public Task<ApiResult<PaginatedResult<UnitDto>>> GetAsync(int PageIndex, int PageSize,string? searchText="");
    public Task<ApiResult<PaginatedResult<UnitDto>>> GetByCompanyAsync(Guid companyId, int PageIndex, int PageSize,string? searchText="");
    public Task<ApiResult<UnitDto>> GetByIdAsync(Guid id);
    public Task<ApiResult<CreateResponseDto>> CreateAsync(UnitDto unit);
    public Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid id);
    public Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(UnitDto unit);
}
