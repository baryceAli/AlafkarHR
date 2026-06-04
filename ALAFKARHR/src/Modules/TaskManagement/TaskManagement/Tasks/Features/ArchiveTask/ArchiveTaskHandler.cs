using TaskManagement.Tasks.Features;

namespace TaskManagement.Tasks.Features.ArchiveTask;

public record ArchiveTaskCommand(Guid Id) : ICommand<ArchiveTaskResult>;
public record ArchiveTaskResult(bool IsSuccess);

public class ArchiveTaskHandler(TaskManagementDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<ArchiveTaskCommand, ArchiveTaskResult>
{
    public async Task<ArchiveTaskResult> Handle(ArchiveTaskCommand command, CancellationToken cancellationToken)
    {
        var userId = TaskFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var task = await dbContext.TaskItems.Include(x => x.History).FirstOrDefaultAsync(x => x.Id == command.Id && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Task not found: {command.Id}");

        task.Archive(userId);
        TaskFeatureHelpers.AddHistoryAndNotification(dbContext, task, userId, "TaskArchived", null, "Archived", task.AssignedToUser);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new ArchiveTaskResult(true);
    }
}
