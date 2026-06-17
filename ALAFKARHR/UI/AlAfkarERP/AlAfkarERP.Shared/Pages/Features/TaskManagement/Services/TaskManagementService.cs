using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.TaskManagement.Dtos;
using SharedWithUI.TaskManagement.Enums;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using TaskWorkflowStatus = SharedWithUI.TaskManagement.Enums.TaskStatus;

namespace AlAfkarERP.Shared.Pages.Features.TaskManagement.Services;

public class TaskManagementService : BaseApiService, ITaskManagementService
{
    private readonly string path;

    public TaskManagementService(HttpClient http, ITokenService tokenService, ApiConfig apiConfig) : base(http, tokenService, apiConfig)
    {
        path = $"api/{apiConfig.Version}/taskmanagement";
    }

    public async Task<ApiResult<PaginatedResult<TaskItemDto>>> GetAsync(int pageIndex, int pageSize, string searchText = "", string? taskNumber = null, string? title = null, string? assignedToUser = null, Guid? departmentId = null, TaskWorkflowStatus? status = null, TaskPriority? priority = null, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var url = $"{path}/tasks?PageIndex={pageIndex}&PageSize={pageSize}&searchText={Uri.EscapeDataString(searchText ?? string.Empty)}";
        if (!string.IsNullOrWhiteSpace(taskNumber)) url += $"&taskNumber={Uri.EscapeDataString(taskNumber)}";
        if (!string.IsNullOrWhiteSpace(title)) url += $"&title={Uri.EscapeDataString(title)}";
        if (!string.IsNullOrWhiteSpace(assignedToUser)) url += $"&assignedToUser={Uri.EscapeDataString(assignedToUser)}";
        if (departmentId.HasValue) url += $"&departmentId={departmentId}";
        if (status.HasValue) url += $"&status={status}";
        if (priority.HasValue) url += $"&priority={priority}";
        if (fromDate.HasValue) url += $"&fromDate={Uri.EscapeDataString(fromDate.Value.ToString("O"))}";
        if (toDate.HasValue) url += $"&toDate={Uri.EscapeDataString(toDate.Value.ToString("O"))}";
        return await SendAsync<PaginatedResult<TaskItemDto>>(new HttpRequestMessage(HttpMethod.Get, url), "taskList");
    }

    public async Task<ApiResult<PaginatedResult<TaskItemDto>>> GetMyTasksAsync(int pageIndex, int pageSize, string searchText = "")
    {
        return await SendAsync<PaginatedResult<TaskItemDto>>(new HttpRequestMessage(HttpMethod.Get, $"{path}/tasks/my?PageIndex={pageIndex}&PageSize={pageSize}&searchText={Uri.EscapeDataString(searchText ?? string.Empty)}"), "taskList");
    }

    public async Task<ApiResult<TaskItemDto>> GetByIdAsync(Guid id)
    {
        return await SendAsync<TaskItemDto>(new HttpRequestMessage(HttpMethod.Get, $"{path}/tasks/{id}"), "task");
    }

    public async Task<ApiResult<CreateResponseDto>> CreateAsync(CreateTaskItemDto task)
    {
        return await SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/tasks")
        {
            Content = JsonContent.Create(new { Task = task })
        }, null);
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(UpdateTaskItemDto task)
    {
        return await SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/tasks")
        {
            Content = JsonContent.Create(new { Task = task })
        }, null);
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> ArchiveAsync(Guid id)
    {
        return await SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Delete, $"{path}/tasks/{id}"), null);
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> AssignAsync(Guid id, AssignTaskDto assignment)
    {
        return await SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/tasks/{id}/assign")
        {
            Content = JsonContent.Create(new { Assignment = assignment })
        }, null);
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> ChangeStatusAsync(Guid id, TaskWorkflowStatus status)
    {
        return await SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/tasks/{id}/status")
        {
            Content = JsonContent.Create(new { TaskWorkflowStatus = new ChangeTaskStatusDto { Id = id, Status = status } })
        }, null);
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> UpdateProgressAsync(Guid id, decimal progressPercentage)
    {
        return await SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/tasks/{id}/progress")
        {
            Content = JsonContent.Create(new { Progress = new UpdateTaskProgressDto { Id = id, ProgressPercentage = progressPercentage } })
        }, null);
    }

    public async Task<ApiResult<CreateResponseDto>> CreateActionAsync(Guid taskId, CreateTaskActionDto action)
    {
        action.TaskId = taskId;
        return await SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/tasks/{taskId}/actions")
        {
            Content = JsonContent.Create(new { Action = action })
        }, null);
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> UpdateActionAsync(Guid taskId, Guid actionId, UpdateTaskActionDto action)
    {
        action.TaskId = taskId;
        action.Id = actionId;
        return await SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/tasks/{taskId}/actions/{actionId}")
        {
            Content = JsonContent.Create(new { Action = action })
        }, null);
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> ToggleActionCompletionAsync(Guid taskId, Guid actionId, bool isCompleted)
    {
        return await SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/tasks/{taskId}/actions/{actionId}/completion")
        {
            Content = JsonContent.Create(new { Action = new ToggleTaskActionCompletionDto { TaskId = taskId, Id = actionId, IsCompleted = isCompleted } })
        }, null);
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> DeleteActionAsync(Guid taskId, Guid actionId)
    {
        return await SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Delete, $"{path}/tasks/{taskId}/actions/{actionId}"), null);
    }

    public async Task<ApiResult<CreateResponseDto>> AddCommentAsync(Guid taskId, string comment)
    {
        return await SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/tasks/{taskId}/comments")
        {
            Content = JsonContent.Create(new { Comment = new CreateTaskCommentDto { TaskId = taskId, Comment = comment } })
        }, null);
    }

    public async Task<ApiResult<CreateResponseDto>> UploadAttachmentAsync(Guid taskId, Stream fileStream, string fileName, string contentType)
    {
        var content = new MultipartFormDataContent();
        var fileContent = new StreamContent(fileStream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        content.Add(fileContent, "file", fileName);

        return await SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/tasks/{taskId}/attachments")
        {
            Content = content
        }, null);
    }

    public async Task<ApiResult<PaginatedResult<TaskNotificationDto>>> GetNotificationsAsync(int pageIndex, int pageSize, bool unreadOnly = false)
    {
        return await SendAsync<PaginatedResult<TaskNotificationDto>>(new HttpRequestMessage(HttpMethod.Get, $"{path}/notifications?PageIndex={pageIndex}&PageSize={pageSize}&unreadOnly={unreadOnly}"), "notifications");
    }

    public async Task<ApiResult<TaskNotificationUnreadCountDto>> GetUnreadNotificationCountAsync()
    {
        return await SendAsync<TaskNotificationUnreadCountDto>(new HttpRequestMessage(HttpMethod.Get, $"{path}/notifications/unread-count"), "unread");
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> MarkNotificationReadAsync(Guid id)
    {
        return await SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/notifications/{id}/read"), null);
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> MarkAllNotificationsReadAsync()
    {
        return await SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/notifications/read-all"), null);
    }

    public async Task<ApiResult<GenerateRecurringTasksResultDto>> GenerateRecurringTasksAsync()
    {
        return await SendAsync<GenerateRecurringTasksResultDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/tasks/generate-recurring"), null);
    }

    public async Task<ApiResult<ProcessTaskRemindersResultDto>> ProcessRemindersAsync()
    {
        return await SendAsync<ProcessTaskRemindersResultDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/tasks/process-reminders"), null);
    }

    public async Task<ApiResult<MarkOverdueTasksResultDto>> MarkOverdueTasksAsync()
    {
        return await SendAsync<MarkOverdueTasksResultDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/tasks/mark-overdue"), null);
    }

    public async Task<ApiResult<RunMyTaskDailyCheckResultDto>> RunMyDailyCheckAsync()
    {
        return await SendAsync<RunMyTaskDailyCheckResultDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/tasks/my/daily-check"), null);
    }

    public async Task<ApiResult<TaskDashboardDto>> GetDashboardAsync(string scope = "mine", Guid? departmentId = null)
    {
        var url = $"{path}/dashboard?scope={scope}";
        if (departmentId.HasValue) url += $"&departmentId={departmentId}";
        return await SendAsync<TaskDashboardDto>(new HttpRequestMessage(HttpMethod.Get, url), "dashboard");
    }

    public async Task<ApiResult<TaskSummaryReportDto>> GetSummaryReportAsync(Guid? departmentId = null, string? userId = null, TaskWorkflowStatus? status = null, TaskPriority? priority = null, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var url = $"{path}/reports/summary?";
        if (departmentId.HasValue) url += $"departmentId={departmentId}&";
        if (!string.IsNullOrWhiteSpace(userId)) url += $"userId={Uri.EscapeDataString(userId)}&";
        if (status.HasValue) url += $"status={status}&";
        if (priority.HasValue) url += $"priority={priority}&";
        if (fromDate.HasValue) url += $"fromDate={Uri.EscapeDataString(fromDate.Value.ToString("O"))}&";
        if (toDate.HasValue) url += $"toDate={Uri.EscapeDataString(toDate.Value.ToString("O"))}&";
        return await SendAsync<TaskSummaryReportDto>(new HttpRequestMessage(HttpMethod.Get, url), "report");
    }

    public async Task<ApiResult<List<EmployeeProductivityReportDto>>> GetEmployeeProductivityReportAsync(Guid? departmentId = null, string? userId = null, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var url = $"{path}/reports/employee-productivity?";
        if (departmentId.HasValue) url += $"departmentId={departmentId}&";
        if (!string.IsNullOrWhiteSpace(userId)) url += $"userId={Uri.EscapeDataString(userId)}&";
        if (fromDate.HasValue) url += $"fromDate={Uri.EscapeDataString(fromDate.Value.ToString("O"))}&";
        if (toDate.HasValue) url += $"toDate={Uri.EscapeDataString(toDate.Value.ToString("O"))}&";
        return await SendAsync<List<EmployeeProductivityReportDto>>(new HttpRequestMessage(HttpMethod.Get, url), "report");
    }

    public async Task<ApiResult<List<DepartmentPerformanceDto>>> GetDepartmentPerformanceReportAsync(Guid? departmentId = null, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var url = $"{path}/reports/department-performance?";
        if (departmentId.HasValue) url += $"departmentId={departmentId}";
        if (fromDate.HasValue) url += $"{(departmentId.HasValue ? "&" : "")}fromDate={Uri.EscapeDataString(fromDate.Value.ToString("O"))}";
        if (toDate.HasValue) url += $"{(departmentId.HasValue || fromDate.HasValue ? "&" : "")}toDate={Uri.EscapeDataString(toDate.Value.ToString("O"))}";
        return await SendAsync<List<DepartmentPerformanceDto>>(new HttpRequestMessage(HttpMethod.Get, url), "report");
    }
}
