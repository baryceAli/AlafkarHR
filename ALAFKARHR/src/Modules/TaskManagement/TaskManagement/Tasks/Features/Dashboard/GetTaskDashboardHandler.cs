using TaskManagement.Tasks.Features;

namespace TaskManagement.Tasks.Features.Dashboard;

public record GetTaskDashboardQuery(string Scope, Guid? DepartmentId) : IQuery<GetTaskDashboardResult>;
public record GetTaskDashboardResult(TaskDashboardDto Dashboard);

public class GetTaskDashboardHandler(TaskManagementDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : IQueryHandler<GetTaskDashboardQuery, GetTaskDashboardResult>
{
    public async Task<GetTaskDashboardResult> Handle(GetTaskDashboardQuery request, CancellationToken cancellationToken)
    {
        var userId = TaskFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var userName = TaskFeatureHelpers.GetCurrentUserName(httpContextAccessor);
        var today = DateTime.UtcNow.Date;
        var weekEnd = today.AddDays(7);

        var query = dbContext.TaskItems.AsNoTracking().Where(x => !x.IsDeleted && !x.IsArchived);
        query = request.Scope.ToLowerInvariant() switch
        {
            "mine" => query.Where(x => x.AssignedToUser == userId.ToString() || x.AssignedToUser == userName),
            "department" when request.DepartmentId.HasValue => query.Where(x => x.DepartmentId == request.DepartmentId.Value),
            _ => TaskFeatureHelpers.ApplyVisibility(query, httpContextAccessor, userId, request.DepartmentId)
        };

        var tasks = await query.ToListAsync(cancellationToken);
        var completed = tasks.Where(x => x.Status == TaskWorkflowStatus.Completed).ToList();
        var total = tasks.Count;

        var dashboard = new TaskDashboardDto
        {
            TotalTasks = total,
            OpenTasks = tasks.Count(x => x.Status is TaskWorkflowStatus.Draft or TaskWorkflowStatus.Assigned or TaskWorkflowStatus.InProgress or TaskWorkflowStatus.OnHold),
            CompletedTasks = completed.Count,
            OverdueTasks = tasks.Count(x => x.Status == TaskWorkflowStatus.Overdue || (x.DueDate.Date < today && x.Status != TaskWorkflowStatus.Completed && x.Status != TaskWorkflowStatus.Cancelled)),
            TasksDueToday = tasks.Count(x => x.DueDate.Date == today),
            TasksDueThisWeek = tasks.Count(x => x.DueDate.Date >= today && x.DueDate.Date <= weekEnd),
            CompletionRate = total == 0 ? 0 : Math.Round(completed.Count * 100m / total, 2),
            AverageCompletionHours = CalculateAverageCompletionHours(completed),
            EmployeeWorkload = tasks
                .GroupBy(x => x.AssignedToUser)
                .Select(g => new EmployeeWorkloadDto
                {
                    UserCode = g.Key,
                    OpenTasks = g.Count(x => x.Status is TaskWorkflowStatus.Draft or TaskWorkflowStatus.Assigned or TaskWorkflowStatus.InProgress or TaskWorkflowStatus.OnHold),
                    CompletedTasks = g.Count(x => x.Status == TaskWorkflowStatus.Completed),
                    OverdueTasks = g.Count(x => x.Status == TaskWorkflowStatus.Overdue || (x.DueDate.Date < today && x.Status != TaskWorkflowStatus.Completed && x.Status != TaskWorkflowStatus.Cancelled))
                })
                .ToList(),
            DepartmentPerformanceRanking = tasks
                .GroupBy(x => x.DepartmentId)
                .Select(g => new DepartmentPerformanceDto
                {
                    DepartmentId = g.Key,
                    TotalTasks = g.Count(),
                    CompletedTasks = g.Count(x => x.Status == TaskWorkflowStatus.Completed),
                    OverdueTasks = g.Count(x => x.Status == TaskWorkflowStatus.Overdue || (x.DueDate.Date < today && x.Status != TaskWorkflowStatus.Completed && x.Status != TaskWorkflowStatus.Cancelled)),
                    CompletionPercentage = g.Any() ? Math.Round(g.Count(x => x.Status == TaskWorkflowStatus.Completed) * 100m / g.Count(), 2) : 0
                })
                .OrderByDescending(x => x.CompletionPercentage)
                .ToList()
        };

        return new GetTaskDashboardResult(dashboard);
    }

    private static decimal CalculateAverageCompletionHours(List<TaskItem> completed)
    {
        if (completed.Count == 0)
            return 0;

        return Math.Round((decimal)completed.Average(x => ((x.CompletedDate ?? x.ModifiedAt ?? x.CreatedAt.Value) - x.CreatedAt.Value).TotalHours), 2);
    }
}
