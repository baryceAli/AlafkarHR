using TaskManagement.Tasks.Features;

namespace TaskManagement.Tasks.Features.MarkOverdueTasks;

public record MarkOverdueTasksCommand : ICommand<MarkOverdueTasksResult>;
public record MarkOverdueTasksResult(int UpdatedCount);

public class MarkOverdueTasksHandler(TaskManagementDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<MarkOverdueTasksCommand, MarkOverdueTasksResult>
{
    public async Task<MarkOverdueTasksResult> Handle(MarkOverdueTasksCommand command, CancellationToken cancellationToken)
    {
        var userId = TaskFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var today = DateTime.UtcNow.Date;
        var tasks = await dbContext.TaskItems.Include(x => x.History)
            .Where(x => !x.IsDeleted
                && x.DueDate.Date < today
                && x.Status != TaskWorkflowStatus.Completed
                && x.Status != TaskWorkflowStatus.Cancelled
                && x.Status != TaskWorkflowStatus.Overdue)
            .ToListAsync(cancellationToken);

        foreach (var task in tasks)
        {
            var oldStatus = task.Status.ToString();
            task.MarkOverdue(userId);
            TaskFeatureHelpers.AddHistoryAndNotification(dbContext, task, userId, "TaskOverdue", oldStatus, TaskWorkflowStatus.Overdue.ToString(), task.AssignedToUserId);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new MarkOverdueTasksResult(tasks.Count);
    }
}
