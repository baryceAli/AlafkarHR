using TaskManagement.Tasks.Features;

namespace TaskManagement.Tasks.Features.AssignTask;

public record AssignTaskCommand(Guid Id, AssignTaskDto Assignment) : ICommand<AssignTaskResult>;
public record AssignTaskResult(bool IsSuccess);

public class AssignTaskCommandValidator : AbstractValidator<AssignTaskCommand>
{
    public AssignTaskCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Assignment.AssignedToUser).NotEmpty();
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
        await TaskFeatureHelpers.EnsureAssignedUserExistsAsync(sender, command.Assignment.AssignedToUser, cancellationToken);
        TaskFeatureHelpers.EnsureDepartment(command.Assignment.DepartmentId);

        var task = await dbContext.TaskItems.FirstOrDefaultAsync(x => x.Id == command.Id && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Task not found: {command.Id}");
        TaskFeatureHelpers.EnsureCanMutateTask(task, httpContextAccessor, userId);

        var oldAssignedTo = task.AssignedToUser;
        task.Assign(command.Assignment.AssignedToUser, userId, command.Assignment.DepartmentId, command.Assignment.StartDate, command.Assignment.DueDate);

        try
        {
            TaskFeatureHelpers.AddHistoryAndNotification(dbContext, task, userId, "TaskAssigned", oldAssignedTo, task.AssignedToUser, task.AssignedToUser);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            await dbContext.Entry(task).ReloadAsync(cancellationToken);
            throw new BadRequestException("This task was modified by another user. Please refresh and try again.", ex.ToString());
        }

        return new AssignTaskResult(true);
    }
}
