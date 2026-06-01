using TaskManagement.Tasks.Features;

namespace TaskManagement.Tasks.Features.ChangeTaskStatus;

public record ChangeTaskStatusCommand(ChangeTaskStatusDto TaskWorkflowStatus) : ICommand<ChangeTaskStatusResult>;
public record ChangeTaskStatusResult(bool IsSuccess);

public class ChangeTaskStatusHandler(TaskManagementDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<ChangeTaskStatusCommand, ChangeTaskStatusResult>
{
    public async Task<ChangeTaskStatusResult> Handle(ChangeTaskStatusCommand command, CancellationToken cancellationToken)
    {
        var userId = TaskFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var task = await dbContext.TaskItems.Include(x => x.History).FirstOrDefaultAsync(x => x.Id == command.TaskWorkflowStatus.Id && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Task not found: {command.TaskWorkflowStatus.Id}");

        var oldStatus = task.Status.ToString();
        task.ChangeStatus(command.TaskWorkflowStatus.Status, userId);

        TaskFeatureHelpers.AddHistoryAndNotification(dbContext, task, userId, "StatusChanged", oldStatus, task.Status.ToString(), task.AssignedToUserId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new ChangeTaskStatusResult(true);
    }
}
