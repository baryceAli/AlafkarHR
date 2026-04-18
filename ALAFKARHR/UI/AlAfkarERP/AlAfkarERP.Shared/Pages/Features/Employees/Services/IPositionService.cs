using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Employees.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.Employees.Services;

public interface IPositionService
{
    public Task<ApiResult<PositionDto>> CreateAsync(PositionDto position);
    public Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(PositionDto position);
    public Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid id);
    public Task<ApiResult<PositionDto>> GetByIdAsync(Guid id);
    public Task<ApiResult<PaginatedResult<PositionDto>>> GetAsync(Guid companyId, int pageIndex, int pageSize);
}
