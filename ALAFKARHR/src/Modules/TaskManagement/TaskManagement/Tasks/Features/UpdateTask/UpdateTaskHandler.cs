using TaskManagement.Tasks.Features;

namespace TaskManagement.Tasks.Features.UpdateTask;

public record UpdateTaskCommand(UpdateTaskItemDto Task) : ICommand<UpdateTaskResult>;
public record UpdateTaskResult(bool IsSuccess);

public class UpdateTaskCommandValidator : AbstractValidator<UpdateTaskCommand>
{
    public UpdateTaskCommandValidator()
    {
        RuleFor(x => x.Task.Id).NotEmpty();
        RuleFor(x => x.Task.Title).NotEmpty().MaximumLength(250);
        RuleFor(x => x.Task.Description).MaximumLength(4000);
        RuleFor(x => x.Task.RecurrenceInterval).GreaterThan(0);
        RuleFor(x => x.Task.RecurrenceFrequency).NotEqual(TaskRecurrenceFrequency.None).When(x => x.Task.IsRecurring);
        RuleFor(x => x.Task.RecurrenceEndDate).NotNull().When(x => x.Task.IsRecurring && x.Task.RecurrenceEndType == TaskRecurrenceEndType.OnDate);
        RuleFor(x => x.Task.RecurrenceMaxOccurrences).NotNull().GreaterThan(0).When(x => x.Task.IsRecurring && x.Task.RecurrenceEndType == TaskRecurrenceEndType.AfterOccurrences);
        RuleFor(x => x.Task.DueDate).Must((command, dueDate) => !command.Task.StartDate.HasValue || dueDate.Date >= command.Task.StartDate.Value.Date)
            .WithMessage("Due date cannot be before start date.");
    }
}

public class UpdateTaskHandler(TaskManagementDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateTaskCommand, UpdateTaskResult>
{
    public async Task<UpdateTaskResult> Handle(UpdateTaskCommand command, CancellationToken cancellationToken)
    {
        var userId = TaskFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var task = await dbContext.TaskItems.FirstOrDefaultAsync(x => x.Id == command.Task.Id && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Task not found: {command.Task.Id}");
        TaskFeatureHelpers.EnsureCanMutateTask(task, httpContextAccessor, userId);

        var oldDueDate = task.DueDate.ToString("O");
        task.Update(command.Task.Title, command.Task.Description, command.Task.Priority, command.Task.StartDate,
            command.Task.DueDate, command.Task.DepartmentId, command.Task.IsRecurring, command.Task.ReminderDate, userId,
            command.Task.RecurrenceFrequency, command.Task.RecurrenceInterval, command.Task.RecurrenceEndType,
            command.Task.RecurrenceEndDate, command.Task.RecurrenceMaxOccurrences);

        try
        {
            TaskFeatureHelpers.AddHistoryAndNotification(dbContext, task, userId, "TaskUpdated", oldDueDate, task.DueDate.ToString("O"), task.AssignedToUser);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            await dbContext.Entry(task).ReloadAsync(cancellationToken);
            throw new BadRequestException("This task was modified by another user. Please refresh and try again.", ex.ToString());
        }

        return new UpdateTaskResult(true);
    }
}
