using TaskManagement.Tasks.Features;

namespace TaskManagement.Tasks.Features.Reports;

public record GetTaskSummaryReportQuery(TaskFilterDto Filter) : IQuery<GetTaskSummaryReportResult>;
public record GetTaskSummaryReportResult(TaskSummaryReportDto Report);
public record GetEmployeeProductivityReportQuery(TaskFilterDto Filter) : IQuery<GetEmployeeProductivityReportResult>;
public record GetEmployeeProductivityReportResult(List<EmployeeProductivityReportDto> Report);
public record GetDepartmentPerformanceReportQuery(TaskFilterDto Filter) : IQuery<GetDepartmentPerformanceReportResult>;
public record GetDepartmentPerformanceReportResult(List<DepartmentPerformanceDto> Report);

public class GetTaskSummaryReportHandler(TaskManagementDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : IQueryHandler<GetTaskSummaryReportQuery, GetTaskSummaryReportResult>
{
    public async Task<GetTaskSummaryReportResult> Handle(GetTaskSummaryReportQuery request, CancellationToken cancellationToken)
    {
        var query = BuildReportQuery(dbContext, httpContextAccessor, request.Filter);
        var tasks = await query.ToListAsync(cancellationToken);
        var today = DateTime.UtcNow.Date;

        return new GetTaskSummaryReportResult(new TaskSummaryReportDto
        {
            TotalTasks = tasks.Count,
            OpenTasks = tasks.Count(x => x.Status is TaskWorkflowStatus.Draft or TaskWorkflowStatus.Assigned or TaskWorkflowStatus.InProgress or TaskWorkflowStatus.OnHold),
            CompletedTasks = tasks.Count(x => x.Status == TaskWorkflowStatus.Completed),
            OverdueTasks = tasks.Count(x => x.Status == TaskWorkflowStatus.Overdue || (x.DueDate.Date < today && x.Status != TaskWorkflowStatus.Completed && x.Status != TaskWorkflowStatus.Cancelled)),
            ByStatus = tasks.GroupBy(x => x.Status).Select(g => new TaskStatusCountDto { Status = g.Key, Count = g.Count() }).ToList(),
            ByPriority = tasks.GroupBy(x => x.Priority).Select(g => new TaskPriorityCountDto { Priority = g.Key, Count = g.Count() }).ToList()
        });
    }

    internal static IQueryable<TaskItem> BuildReportQuery(TaskManagementDbContext dbContext, IHttpContextAccessor httpContextAccessor, TaskFilterDto filter)
    {
        var currentUserId = TaskFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var query = dbContext.TaskItems.AsNoTracking().Where(x => !x.IsDeleted && !x.IsArchived);
        query = TaskFeatureHelpers.ApplyVisibility(query, httpContextAccessor, currentUserId, filter.DepartmentId);
        return TaskFeatureHelpers.ApplyFilters(query, filter);
    }
}

public class GetEmployeeProductivityReportHandler(TaskManagementDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : IQueryHandler<GetEmployeeProductivityReportQuery, GetEmployeeProductivityReportResult>
{
    public async Task<GetEmployeeProductivityReportResult> Handle(GetEmployeeProductivityReportQuery request, CancellationToken cancellationToken)
    {
        var tasks = await GetTaskSummaryReportHandler.BuildReportQuery(dbContext, httpContextAccessor, request.Filter).ToListAsync(cancellationToken);
        var report = tasks.GroupBy(x => x.AssignedToUser)
            .Select(g =>
            {
                var completed = g.Where(x => x.Status == TaskWorkflowStatus.Completed).ToList();
                return new EmployeeProductivityReportDto
                {
                    UserCode = g.Key,
                    AssignedTasks = g.Count(),
                    CompletedTasks = completed.Count,
                    CompletionRate = g.Any() ? Math.Round(completed.Count * 100m / g.Count(), 2) : 0,
                    AverageCompletionHours = completed.Count == 0 ? 0 : Math.Round((decimal)completed.Average(x => ((x.CompletedDate ?? x.ModifiedAt ?? x.CreatedAt.Value) - x.CreatedAt.Value).TotalHours), 2)
                };
            })
            .ToList();

        return new GetEmployeeProductivityReportResult(report);
    }
}

public class GetDepartmentPerformanceReportHandler(TaskManagementDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : IQueryHandler<GetDepartmentPerformanceReportQuery, GetDepartmentPerformanceReportResult>
{
    public async Task<GetDepartmentPerformanceReportResult> Handle(GetDepartmentPerformanceReportQuery request, CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;
        var tasks = await GetTaskSummaryReportHandler.BuildReportQuery(dbContext, httpContextAccessor, request.Filter).ToListAsync(cancellationToken);
        var report = tasks.GroupBy(x => x.DepartmentId)
            .Select(g => new DepartmentPerformanceDto
            {
                DepartmentId = g.Key,
                TotalTasks = g.Count(),
                CompletedTasks = g.Count(x => x.Status == TaskWorkflowStatus.Completed),
                OverdueTasks = g.Count(x => x.Status == TaskWorkflowStatus.Overdue || (x.DueDate.Date < today && x.Status != TaskWorkflowStatus.Completed && x.Status != TaskWorkflowStatus.Cancelled)),
                CompletionPercentage = g.Any() ? Math.Round(g.Count(x => x.Status == TaskWorkflowStatus.Completed) * 100m / g.Count(), 2) : 0
            })
            .OrderByDescending(x => x.CompletionPercentage)
            .ToList();

        return new GetDepartmentPerformanceReportResult(report);
    }
}
