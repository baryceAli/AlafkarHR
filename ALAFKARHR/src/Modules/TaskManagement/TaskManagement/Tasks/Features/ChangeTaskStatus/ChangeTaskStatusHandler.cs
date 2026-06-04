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
        var task = await dbContext.TaskItems.FirstOrDefaultAsync(x => x.Id == command.TaskWorkflowStatus.Id && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Task not found: {command.TaskWorkflowStatus.Id}");

        var oldStatus = task.Status.ToString();
        task.ChangeStatus(command.TaskWorkflowStatus.Status, userId);

        try
        {
            TaskFeatureHelpers.AddHistoryAndNotification(dbContext, task, userId, "StatusChanged", oldStatus, task.Status.ToString(), task.AssignedToUser);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            await dbContext.Entry(task).ReloadAsync(cancellationToken);
            throw new BadRequestException("This task was modified by another user. Please refresh and try again.", ex.ToString());
        }

        return new ChangeTaskStatusResult(true);
    }
}
