using TaskManagement.Tasks.Features;
using TaskManagement.Tasks.Services;

namespace TaskManagement.Tasks.Features.CreateTask;

public record CreateTaskCommand(CreateTaskItemDto Task) : ICommand<CreateTaskResult>;
public record CreateTaskResult(Guid Id);

public class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
        RuleFor(x => x.Task.Title).NotEmpty().MaximumLength(250);
        RuleFor(x => x.Task.Description).MaximumLength(4000);
        RuleFor(x => x.Task.AssignedToUser).NotEmpty();
        RuleFor(x => x.Task.RecurrenceInterval).GreaterThan(0);
        RuleFor(x => x.Task.RecurrenceFrequency).NotEqual(TaskRecurrenceFrequency.None).When(x => x.Task.IsRecurring);
        RuleFor(x => x.Task.RecurrenceEndDate).NotNull().When(x => x.Task.IsRecurring && x.Task.RecurrenceEndType == TaskRecurrenceEndType.OnDate);
        RuleFor(x => x.Task.RecurrenceMaxOccurrences).NotNull().GreaterThan(0).When(x => x.Task.IsRecurring && x.Task.RecurrenceEndType == TaskRecurrenceEndType.AfterOccurrences);
        RuleFor(x => x.Task.DueDate).Must((command, dueDate) => !command.Task.StartDate.HasValue || dueDate.Date >= command.Task.StartDate.Value.Date)
            .WithMessage("Due date cannot be before start date.");
        RuleForEach(x => x.Task.Actions).ChildRules(action =>
        {
            action.RuleFor(x => x.Title).NotEmpty().MaximumLength(500);
        });
        RuleFor(x => x.Task.Actions)
            .Must((command, actions) => actions is null || actions.All(action => !action.ExpectedCompletionAt.HasValue || !command.Task.StartDate.HasValue || action.ExpectedCompletionAt.Value.Date >= command.Task.StartDate.Value.Date))
            .WithMessage("Action expected completion cannot be before task start date.");
        RuleFor(x => x.Task.Actions)
            .Must((command, actions) => actions is null || actions.All(action => !action.ExpectedCompletionAt.HasValue || action.ExpectedCompletionAt.Value.Date <= command.Task.DueDate.Date))
            .WithMessage("Action expected completion cannot be after task due date.");
    }
}

public class CreateTaskHandler(
    TaskManagementDbContext dbContext,
    IHttpContextAccessor httpContextAccessor,
    ISender sender,
    ITaskNumberGenerator taskNumberGenerator)
    : ICommandHandler<CreateTaskCommand, CreateTaskResult>
{
    public async Task<CreateTaskResult> Handle(CreateTaskCommand command, CancellationToken cancellationToken)
    {
        var currentUserId = TaskFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var currentUserName = TaskFeatureHelpers.GetCurrentUserName(httpContextAccessor);
        await TaskFeatureHelpers.EnsureAssignedUserExistsAsync(sender, command.Task.AssignedToUser, cancellationToken);

        var taskNumber = await taskNumberGenerator.GenerateAsync(cancellationToken);
        var task = TaskItem.Create(taskNumber, command.Task.Title, command.Task.Description, command.Task.Priority,
            command.Task.StartDate, command.Task.DueDate, currentUserId, command.Task.AssignedToUser, currentUserId,
            command.Task.DepartmentId, command.Task.IsRecurring, command.Task.ReminderDate, command.Task.RecurrenceFrequency,
            command.Task.RecurrenceInterval, command.Task.RecurrenceEndType, command.Task.RecurrenceEndDate,
            command.Task.RecurrenceMaxOccurrences);

        foreach (var actionDto in command.Task.Actions ?? [])
        {
            var action = TaskActionItem.Create(task.Id, actionDto.Title, actionDto.ExpectedCompletionAt, currentUserId, currentUserName);
            task.AddAction(action);
        }

        if (task.Actions.Count > 0)
            task.RecalculateProgressFromActions(currentUserId);
        TaskFeatureHelpers.AddHistoryAndNotification(dbContext, task, currentUserId, "TaskCreated", null, task.AssignedToUser, task.AssignedToUser);

        await dbContext.TaskItems.AddAsync(task, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateTaskResult(task.Id);
    }
}
