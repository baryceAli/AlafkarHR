namespace TaskManagement.Tasks.Features.TaskActions;

public record UpdateTaskActionCommand(UpdateTaskActionDto Action) : ICommand<UpdateTaskActionResult>;
public record UpdateTaskActionResult(bool IsSuccess);

public class UpdateTaskActionCommandValidator : AbstractValidator<UpdateTaskActionCommand>
{
    public UpdateTaskActionCommandValidator()
    {
        RuleFor(x => x.Action.TaskId).NotEmpty();
        RuleFor(x => x.Action.Id).NotEmpty();
        RuleFor(x => x.Action.Title).NotEmpty().MaximumLength(500);
    }
}

public class UpdateTaskActionHandler(TaskManagementDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateTaskActionCommand, UpdateTaskActionResult>
{
    public async Task<UpdateTaskActionResult> Handle(UpdateTaskActionCommand command, CancellationToken cancellationToken)
    {
        var userId = TaskFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var task = await dbContext.TaskItems
            .Include(x => x.Actions)
            .FirstOrDefaultAsync(x => x.Id == command.Action.TaskId && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Task not found: {command.Action.TaskId}");

        var action = task.Actions.FirstOrDefault(x => x.Id == command.Action.Id && !x.IsDeleted)
            ?? throw new NotFoundException($"Task action not found: {command.Action.Id}");
        TaskFeatureHelpers.EnsureCanMutateAction(action, httpContextAccessor, userId);

        var oldValue = action.Title;
        action.Update(command.Action.Title, command.Action.ExpectedCompletionAt, userId);
        TaskFeatureHelpers.AddHistoryAndNotification(dbContext, task, userId, "TaskActionUpdated", oldValue, action.Title, task.AssignedToUser);

        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateTaskActionResult(true);
    }
}
