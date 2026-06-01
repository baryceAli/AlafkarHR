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
        RuleFor(x => x.Task.AssignedToUserId).NotEmpty();
        RuleFor(x => x.Task.DepartmentId).NotEmpty();
        RuleFor(x => x.Task.DueDate).Must((command, dueDate) => !command.Task.StartDate.HasValue || dueDate.Date >= command.Task.StartDate.Value.Date)
            .WithMessage("Due date cannot be before start date.");
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
        await TaskFeatureHelpers.EnsureAssignedUserExistsAsync(sender, command.Task.AssignedToUserId, cancellationToken);
        TaskFeatureHelpers.EnsureDepartment(command.Task.DepartmentId);

        var taskNumber = await taskNumberGenerator.GenerateAsync(cancellationToken);
        var task = TaskItem.Create(taskNumber, command.Task.Title, command.Task.Description, command.Task.Priority,
            command.Task.StartDate, command.Task.DueDate, currentUserId, command.Task.AssignedToUserId, currentUserId,
            command.Task.DepartmentId, command.Task.IsRecurring, command.Task.ReminderDate);

        TaskFeatureHelpers.AddHistoryAndNotification(dbContext, task, currentUserId, "TaskCreated", null, task.AssignedToUserId.ToString(), task.AssignedToUserId);

        await dbContext.TaskItems.AddAsync(task, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateTaskResult(task.Id);
    }
}
