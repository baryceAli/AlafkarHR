namespace TaskManagement.Tasks.Features.TaskActions;

public record DeleteTaskActionCommand(Guid TaskId, Guid ActionId) : ICommand<DeleteTaskActionResult>;
public record DeleteTaskActionResult(bool IsSuccess);

public class DeleteTaskActionHandler(TaskManagementDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<DeleteTaskActionCommand, DeleteTaskActionResult>
{
    public async Task<DeleteTaskActionResult> Handle(DeleteTaskActionCommand command, CancellationToken cancellationToken)
    {
        var userId = TaskFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var task = await dbContext.TaskItems
            .Include(x => x.Actions)
            .FirstOrDefaultAsync(x => x.Id == command.TaskId && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Task not found: {command.TaskId}");

        var action = task.Actions.FirstOrDefault(x => x.Id == command.ActionId && !x.IsDeleted)
            ?? throw new NotFoundException($"Task action not found: {command.ActionId}");
        TaskFeatureHelpers.EnsureCanMutateAction(action, httpContextAccessor, userId);

        action.Remove(userId);
        task.RecalculateProgressFromActions(userId);
        TaskFeatureHelpers.AddHistoryAndNotification(dbContext, task, userId, "TaskActionDeleted", action.Title, null, task.AssignedToUser);

        await dbContext.SaveChangesAsync(cancellationToken);
        return new DeleteTaskActionResult(true);
    }
}
