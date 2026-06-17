namespace TaskManagement.Tasks.Features.TaskActions;

public record ToggleTaskActionCompletionCommand(ToggleTaskActionCompletionDto Action) : ICommand<ToggleTaskActionCompletionResult>;
public record ToggleTaskActionCompletionResult(bool IsSuccess);

public class ToggleTaskActionCompletionCommandValidator : AbstractValidator<ToggleTaskActionCompletionCommand>
{
    public ToggleTaskActionCompletionCommandValidator()
    {
        RuleFor(x => x.Action.TaskId).NotEmpty();
        RuleFor(x => x.Action.Id).NotEmpty();
    }
}

public class ToggleTaskActionCompletionHandler(TaskManagementDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<ToggleTaskActionCompletionCommand, ToggleTaskActionCompletionResult>
{
    public async Task<ToggleTaskActionCompletionResult> Handle(ToggleTaskActionCompletionCommand command, CancellationToken cancellationToken)
    {
        var userId = TaskFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var task = await dbContext.TaskItems
            .Include(x => x.Actions)
            .FirstOrDefaultAsync(x => x.Id == command.Action.TaskId && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Task not found: {command.Action.TaskId}");

        var action = task.Actions.FirstOrDefault(x => x.Id == command.Action.Id && !x.IsDeleted)
            ?? throw new NotFoundException($"Task action not found: {command.Action.Id}");
        TaskFeatureHelpers.EnsureCanMutateAction(action, httpContextAccessor, userId);

        var oldValue = action.IsCompleted.ToString();
        action.SetCompletion(command.Action.IsCompleted, userId);
        task.RecalculateProgressFromActions(userId);
        TaskFeatureHelpers.AddHistoryAndNotification(dbContext, task, userId, "TaskActionCompletionChanged", oldValue, action.IsCompleted.ToString(), task.AssignedToUser);

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ToggleTaskActionCompletionResult(true);
    }
}
