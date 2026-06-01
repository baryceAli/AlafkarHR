using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.TaskManagement.Dtos;
using SharedWithUI.TaskManagement.Enums;
using System.Net.Http.Json;
using TaskWorkflowStatus = SharedWithUI.TaskManagement.Enums.TaskStatus;

namespace AlAfkarERP.Shared.Pages.Features.TaskManagement.Services;

public class TaskManagementService : BaseApiService, ITaskManagementService
{
    private readonly string path;

    public TaskManagementService(HttpClient http, ApiConfig apiConfig) : base(http)
    {
        path = $"api/{apiConfig.Version}/taskmanagement";
    }

    public async Task<ApiResult<PaginatedResult<TaskItemDto>>> GetAsync(int pageIndex, int pageSize, string searchText = "", Guid? departmentId = null, TaskWorkflowStatus? status = null, TaskPriority? priority = null)
    {
        var url = $"{path}/tasks?PageIndex={pageIndex}&PageSize={pageSize}&searchText={Uri.EscapeDataString(searchText ?? string.Empty)}";
        if (departmentId.HasValue) url += $"&departmentId={departmentId}";
        if (status.HasValue) url += $"&status={status}";
        if (priority.HasValue) url += $"&priority={priority}";
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

    public async Task<ApiResult<CreateResponseDto>> AddCommentAsync(Guid taskId, string comment)
    {
        return await SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/tasks/{taskId}/comments")
        {
            Content = JsonContent.Create(new { Comment = new CreateTaskCommentDto { TaskId = taskId, Comment = comment } })
        }, null);
    }

    public async Task<ApiResult<TaskDashboardDto>> GetDashboardAsync(string scope = "mine", Guid? departmentId = null)
    {
        var url = $"{path}/dashboard?scope={scope}";
        if (departmentId.HasValue) url += $"&departmentId={departmentId}";
        return await SendAsync<TaskDashboardDto>(new HttpRequestMessage(HttpMethod.Get, url), "dashboard");
    }

    public async Task<ApiResult<TaskSummaryReportDto>> GetSummaryReportAsync(Guid? departmentId = null, Guid? userId = null, TaskWorkflowStatus? status = null, TaskPriority? priority = null)
    {
        var url = $"{path}/reports/summary?";
        if (departmentId.HasValue) url += $"departmentId={departmentId}&";
        if (userId.HasValue) url += $"userId={userId}&";
        if (status.HasValue) url += $"status={status}&";
        if (priority.HasValue) url += $"priority={priority}&";
        return await SendAsync<TaskSummaryReportDto>(new HttpRequestMessage(HttpMethod.Get, url), "report");
    }

    public async Task<ApiResult<List<EmployeeProductivityReportDto>>> GetEmployeeProductivityReportAsync(Guid? departmentId = null, Guid? userId = null)
    {
        var url = $"{path}/reports/employee-productivity?";
        if (departmentId.HasValue) url += $"departmentId={departmentId}&";
        if (userId.HasValue) url += $"userId={userId}&";
        return await SendAsync<List<EmployeeProductivityReportDto>>(new HttpRequestMessage(HttpMethod.Get, url), "report");
    }

    public async Task<ApiResult<List<DepartmentPerformanceDto>>> GetDepartmentPerformanceReportAsync(Guid? departmentId = null)
    {
        var url = $"{path}/reports/department-performance?";
        if (departmentId.HasValue) url += $"departmentId={departmentId}";
        return await SendAsync<List<DepartmentPerformanceDto>>(new HttpRequestMessage(HttpMethod.Get, url), "report");
    }
}
