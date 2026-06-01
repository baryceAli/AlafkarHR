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
        var task = await dbContext.TaskItems.Include(x => x.History).FirstOrDefaultAsync(x => x.Id == command.Progress.Id && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Task not found: {command.Progress.Id}");

        var oldProgress = task.ProgressPercentage.ToString("0.##");
        task.UpdateProgress(command.Progress.ProgressPercentage, userId);
        TaskFeatureHelpers.AddHistoryAndNotification(dbContext, task, userId, "ProgressUpdated", oldProgress, task.ProgressPercentage.ToString("0.##"), task.AssignedToUserId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateTaskProgressResult(true);
    }
}
