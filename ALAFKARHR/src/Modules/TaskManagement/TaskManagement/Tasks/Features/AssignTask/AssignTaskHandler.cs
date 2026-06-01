using TaskManagement.Tasks.Features;

namespace TaskManagement.Tasks.Features.AssignTask;

public record AssignTaskCommand(Guid Id, AssignTaskDto Assignment) : ICommand<AssignTaskResult>;
public record AssignTaskResult(bool IsSuccess);

public class AssignTaskCommandValidator : AbstractValidator<AssignTaskCommand>
{
    public AssignTaskCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Assignment.AssignedToUserId).NotEmpty();
        RuleFor(x => x.Assignment.DepartmentId).NotEmpty();
        RuleFor(x => x.Assignment.DueDate).Must((command, dueDate) => !command.Assignment.StartDate.HasValue || dueDate.Date >= command.Assignment.StartDate.Value.Date)
            .WithMessage("Due date cannot be before start date.");
    }
}

public class AssignTaskHandler(TaskManagementDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<AssignTaskCommand, AssignTaskResult>
{
    public async Task<AssignTaskResult> Handle(AssignTaskCommand command, CancellationToken cancellationToken)
    {
        var userId = TaskFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        await TaskFeatureHelpers.EnsureAssignedUserExistsAsync(sender, command.Assignment.AssignedToUserId, cancellationToken);
        TaskFeatureHelpers.EnsureDepartment(command.Assignment.DepartmentId);

        var task = await dbContext.TaskItems.Include(x => x.History).FirstOrDefaultAsync(x => x.Id == command.Id && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Task not found: {command.Id}");

        var oldAssignedTo = task.AssignedToUserId.ToString();
        task.Assign(command.Assignment.AssignedToUserId, userId, command.Assignment.DepartmentId, command.Assignment.StartDate, command.Assignment.DueDate);

        TaskFeatureHelpers.AddHistoryAndNotification(dbContext, task, userId, "TaskAssigned", oldAssignedTo, task.AssignedToUserId.ToString(), task.AssignedToUserId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new AssignTaskResult(true);
    }
}
