using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Employees.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.Employees.Services;

public interface IEmployeeService
{
    public Task<ApiResult<EmployeeDto>> GreateAsync(EmployeeDto employee);
    public Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(EmployeeDto employee);
    public Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid id);

    public Task<ApiResult<UpdateDeleteResponseDto>> ChangePositionAsync(ChangePositionDto changePosition);
    public Task<ApiResult<UpdateDeleteResponseDto>> TerminateEmployeeAsync(TerminateEmployeeDto terminateEmployee);
    public Task<ApiResult<UpdateDeleteResponseDto>> TransferDepartmentAsync(TransferDepartmentDto transferDepartment);

    public Task<ApiResult<EmployeeDto>> GetByIdAsync(Guid id);
    public Task<ApiResult<PaginatedResult<EmployeeDto>>> GetAsync(int pageIndex, int pageSize);
    public Task<ApiResult<PaginatedResult<EmployeeDto>>> GetByCompanyIdAsync(Guid companyId, int pageIndex, int pageSize);
    public Task<ApiResult<PaginatedResult<EmployeeDto>>> GetByBranchAsync(Guid branchId, int pageIndex, int pageSize);
    public Task<ApiResult<PaginatedResult<EmployeeDto>>> GetByAdministrationAsync(Guid administrationId, int pageIndex, int pageSize);
    public Task<ApiResult<PaginatedResult<EmployeeDto>>> GetByDepartmentAsync(Guid departmentId, int pageIndex, int pageSize);
    public Task<ApiResult<PaginatedResult<EmployeeDto>>> GetByPositionAsync(Guid positionId, int pageIndex, int pageSize);
    

}
