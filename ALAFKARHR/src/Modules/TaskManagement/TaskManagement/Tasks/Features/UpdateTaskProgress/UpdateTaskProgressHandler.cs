using TaskManagement.Tasks.Features;

namespace TaskManagement.Tasks.Features.UpdateTaskProgress;

public record UpdateTaskProgressCommand(UpdateTaskProgressDto Progress) : ICommand<UpdateTaskProgressResult>;
public record UpdateTaskProgressResult(bool IsSuccess);

public class UpdateTaskProgressCommandValidator : AbstractValidator<UpdateTaskProgressCommand>
{
    public UpdateTaskProgressCommandValidator()
    {
        RuleFor(x => x.Progress.ProgressPercentage).InclusiveBetween(0, 100);
    }
}

public class UpdateTaskProgressHandler(TaskManagementDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateTaskProgressCommand, UpdateTaskProgressResult>
{
    public async Task<UpdateTaskProgressResult> Handle(UpdateTaskProgressCommand command, CancellationToken cancellationToken)
    {
        var userId = TaskFeatureHelpers.GetCurrentUserId(httpContextAccessor);

        // Query without including history first to avoid concurrency issues with collections
        var task = await dbContext.TaskItems.FirstOrDefaultAsync(x => x.Id == command.Progress.Id && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Task not found: {command.Progress.Id}");

        var oldProgress = task.ProgressPercentage.ToString("0.##");
        task.UpdateProgress(command.Progress.ProgressPercentage, userId);

        try
        {
            TaskFeatureHelpers.AddHistoryAndNotification(dbContext, task, userId, "ProgressUpdated", oldProgress, task.ProgressPercentage.ToString("0.##"), task.AssignedToUser);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            // Refresh the entity from the database and try again
            await dbContext.Entry(task).ReloadAsync(cancellationToken);
            throw new BadRequestException("This task was modified by another user. Please refresh and try again.", ex.ToString());
        }

        return new UpdateTaskProgressResult(true);
    }
}
