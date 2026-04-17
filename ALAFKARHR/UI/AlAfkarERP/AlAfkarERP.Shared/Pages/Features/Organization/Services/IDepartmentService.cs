using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Organization.Dtos;
using AlAfkarERP.Shared.Pages.Reuable2;

namespace AlAfkarERP.Shared.Pages.Features.Company.Services;

public interface IDepartmentService
{
    Task<ApiResult<DepartmentDto>> CreateAsync(DepartmentDto department);
    Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(DepartmentDto department);
    Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid Id);
    Task<ApiResult<DepartmentDto>> GetByIdAsync(Guid Id);
    Task<ApiResult<PaginatedResult<DepartmentDto>>> GetAsync(int pageIndex, int pageSize);
    Task<ApiResult<PaginatedResult<DepartmentDto>>> GetByAdministrationAsync(Guid AdministrationId,int pageIndex, int pageSize);
    Task<ApiResult<List<DepartmentDto>>> GetByAdministrationAsync(Guid administrationId);
}
