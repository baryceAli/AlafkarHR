using AlAfkarERP.Shared.Dtos;
using SharedWithUI.TaskManagement.Dtos;
using SharedWithUI.TaskManagement.Enums;
using TaskWorkflowStatus = SharedWithUI.TaskManagement.Enums.TaskStatus;

namespace AlAfkarERP.Shared.Pages.Features.TaskManagement.Services;

public interface ITaskManagementService
{
    Task<ApiResult<PaginatedResult<TaskItemDto>>> GetAsync(int pageIndex, int pageSize, string searchText = "", Guid? departmentId = null, TaskWorkflowStatus? status = null, TaskPriority? priority = null);
    Task<ApiResult<PaginatedResult<TaskItemDto>>> GetMyTasksAsync(int pageIndex, int pageSize, string searchText = "");
    Task<ApiResult<TaskItemDto>> GetByIdAsync(Guid id);
    Task<ApiResult<CreateResponseDto>> CreateAsync(CreateTaskItemDto task);
    Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(UpdateTaskItemDto task);
    Task<ApiResult<UpdateDeleteResponseDto>> ArchiveAsync(Guid id);
    Task<ApiResult<UpdateDeleteResponseDto>> AssignAsync(Guid id, AssignTaskDto assignment);
    Task<ApiResult<UpdateDeleteResponseDto>> ChangeStatusAsync(Guid id, TaskWorkflowStatus status);
    Task<ApiResult<UpdateDeleteResponseDto>> UpdateProgressAsync(Guid id, decimal progressPercentage);
    Task<ApiResult<CreateResponseDto>> AddCommentAsync(Guid taskId, string comment);
    Task<ApiResult<TaskDashboardDto>> GetDashboardAsync(string scope = "mine", Guid? departmentId = null);
    Task<ApiResult<TaskSummaryReportDto>> GetSummaryReportAsync(Guid? departmentId = null, Guid? userId = null, TaskWorkflowStatus? status = null, TaskPriority? priority = null);
    Task<ApiResult<List<EmployeeProductivityReportDto>>> GetEmployeeProductivityReportAsync(Guid? departmentId = null, Guid? userId = null);
    Task<ApiResult<List<DepartmentPerformanceDto>>> GetDepartmentPerformanceReportAsync(Guid? departmentId = null);
}
