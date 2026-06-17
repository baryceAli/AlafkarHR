namespace TaskManagement.Tasks.Features.TaskActions;

public record CreateTaskActionCommand(CreateTaskActionDto Action) : ICommand<CreateTaskActionResult>;
public record CreateTaskActionResult(Guid Id);

public class CreateTaskActionCommandValidator : AbstractValidator<CreateTaskActionCommand>
{
    public CreateTaskActionCommandValidator()
    {
        RuleFor(x => x.Action.TaskId).NotEmpty();
        RuleFor(x => x.Action.Title).NotEmpty().MaximumLength(500);
    }
}

public class CreateTaskActionHandler(TaskManagementDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CreateTaskActionCommand, CreateTaskActionResult>
{
    public async Task<CreateTaskActionResult> Handle(CreateTaskActionCommand command, CancellationToken cancellationToken)
    {
        var userId = TaskFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var userName = TaskFeatureHelpers.GetCurrentUserName(httpContextAccessor);
        var task = await dbContext.TaskItems
            .Include(x => x.Actions)
            .FirstOrDefaultAsync(x => x.Id == command.Action.TaskId && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Task not found: {command.Action.TaskId}");

        TaskFeatureHelpers.EnsureCanMutateTask(task, httpContextAccessor, userId);

        var action = TaskActionItem.Create(task.Id, command.Action.Title, command.Action.ExpectedCompletionAt, userId, userName);
        task.AddAction(action);
        task.RecalculateProgressFromActions(userId);
        TaskFeatureHelpers.AddHistoryAndNotification(dbContext, task, userId, "TaskActionCreated", null, action.Title, task.AssignedToUser);

        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateTaskActionResult(action.Id);
    }
}
