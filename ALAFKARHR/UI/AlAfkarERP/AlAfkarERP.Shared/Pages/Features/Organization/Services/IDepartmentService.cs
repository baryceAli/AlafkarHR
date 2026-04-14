using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Pages.Features.Company.Dtos;
using AlAfkarERP.Shared.Pages.Reuable;

namespace AlAfkarERP.Shared.Pages.Features.Company.Services;

public interface IDepartmentService
{
    Task<ApiResult<DepartmentDto>> CreateAsync(DepartmentDto department);
    Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(DepartmentDto department);
    Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid Id);
    Task<ApiResult<DepartmentDto>> GetByIdAsync(Guid Id);
    Task<ApiResult<PagedResult<DepartmentDto>>> GetAsync(int pageIndex, int pageSize);
    Task<ApiResult<List<DepartmentDto>>> GetByAdministrationAsync(Guid administrationId);
}
