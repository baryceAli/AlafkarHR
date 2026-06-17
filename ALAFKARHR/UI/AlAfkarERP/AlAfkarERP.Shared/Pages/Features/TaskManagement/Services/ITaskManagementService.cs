using AlAfkarERP.Shared.Dtos;
using SharedWithUI.TaskManagement.Dtos;
using SharedWithUI.TaskManagement.Enums;
using TaskWorkflowStatus = SharedWithUI.TaskManagement.Enums.TaskStatus;

namespace AlAfkarERP.Shared.Pages.Features.TaskManagement.Services;

public interface ITaskManagementService
{
    Task<ApiResult<PaginatedResult<TaskItemDto>>> GetAsync(int pageIndex, int pageSize, string searchText = "", string? taskNumber = null, string? title = null, string? assignedToUser = null, Guid? departmentId = null, TaskWorkflowStatus? status = null, TaskPriority? priority = null, DateTime? fromDate = null, DateTime? toDate = null);
    Task<ApiResult<PaginatedResult<TaskItemDto>>> GetMyTasksAsync(int pageIndex, int pageSize, string searchText = "");
    Task<ApiResult<TaskItemDto>> GetByIdAsync(Guid id);
    Task<ApiResult<CreateResponseDto>> CreateAsync(CreateTaskItemDto task);
    Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(UpdateTaskItemDto task);
    Task<ApiResult<UpdateDeleteResponseDto>> ArchiveAsync(Guid id);
    Task<ApiResult<UpdateDeleteResponseDto>> AssignAsync(Guid id, AssignTaskDto assignment);
    Task<ApiResult<UpdateDeleteResponseDto>> ChangeStatusAsync(Guid id, TaskWorkflowStatus status);
    Task<ApiResult<UpdateDeleteResponseDto>> UpdateProgressAsync(Guid id, decimal progressPercentage);
    Task<ApiResult<CreateResponseDto>> CreateActionAsync(Guid taskId, CreateTaskActionDto action);
    Task<ApiResult<UpdateDeleteResponseDto>> UpdateActionAsync(Guid taskId, Guid actionId, UpdateTaskActionDto action);
    Task<ApiResult<UpdateDeleteResponseDto>> ToggleActionCompletionAsync(Guid taskId, Guid actionId, bool isCompleted);
    Task<ApiResult<UpdateDeleteResponseDto>> DeleteActionAsync(Guid taskId, Guid actionId);
    Task<ApiResult<CreateResponseDto>> AddCommentAsync(Guid taskId, string comment);
    Task<ApiResult<CreateResponseDto>> UploadAttachmentAsync(Guid taskId, Stream fileStream, string fileName, string contentType);
    Task<ApiResult<PaginatedResult<TaskNotificationDto>>> GetNotificationsAsync(int pageIndex, int pageSize, bool unreadOnly = false);
    Task<ApiResult<TaskNotificationUnreadCountDto>> GetUnreadNotificationCountAsync();
    Task<ApiResult<UpdateDeleteResponseDto>> MarkNotificationReadAsync(Guid id);
    Task<ApiResult<UpdateDeleteResponseDto>> MarkAllNotificationsReadAsync();
    Task<ApiResult<GenerateRecurringTasksResultDto>> GenerateRecurringTasksAsync();
    Task<ApiResult<ProcessTaskRemindersResultDto>> ProcessRemindersAsync();
    Task<ApiResult<MarkOverdueTasksResultDto>> MarkOverdueTasksAsync();
    Task<ApiResult<RunMyTaskDailyCheckResultDto>> RunMyDailyCheckAsync();
    Task<ApiResult<TaskDashboardDto>> GetDashboardAsync(string scope = "mine", Guid? departmentId = null);
    Task<ApiResult<TaskSummaryReportDto>> GetSummaryReportAsync(Guid? departmentId = null, string? userId = null, TaskWorkflowStatus? status = null, TaskPriority? priority = null, DateTime? fromDate = null, DateTime? toDate = null);
    Task<ApiResult<List<EmployeeProductivityReportDto>>> GetEmployeeProductivityReportAsync(Guid? departmentId = null, string? userId = null, DateTime? fromDate = null, DateTime? toDate = null);
    Task<ApiResult<List<DepartmentPerformanceDto>>> GetDepartmentPerformanceReportAsync(Guid? departmentId = null, DateTime? fromDate = null, DateTime? toDate = null);
}
