using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Employees.Dtos;
using SharedWithUI.Employees.Enums;

namespace AlAfkarERP.Shared.Pages.Features.Employees.Services;

public interface IEmployeeTeamService
{
    Task<ApiResult<PaginatedResult<EmployeeTeamDto>>> GetAsync(Guid companyId, int pageIndex, int pageSize, string? searchText = null, EmployeeTeamCategory? category = null, bool? isActive = null);
    Task<ApiResult<PaginatedResult<EmployeeTeamDto>>> GetProjectTeamsAsync(Guid companyId);
    Task<ApiResult<EmployeeTeamDto>> GetByIdAsync(Guid id);
    Task<ApiResult<EmployeeTeamDto>> CreateAsync(EmployeeTeamDto team);
    Task<ApiResult<EmployeeTeamDto>> CreateProjectTeamAsync(EmployeeTeamDto team);
    Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(EmployeeTeamDto team);
    Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid id);
}
