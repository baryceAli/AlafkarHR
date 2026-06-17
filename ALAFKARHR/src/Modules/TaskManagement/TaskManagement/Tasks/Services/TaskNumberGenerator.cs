namespace TaskManagement.Tasks.Services;

public class TaskNumberGenerator(TaskManagementDbContext dbContext) : ITaskNumberGenerator
{
    public async Task<string> GenerateAsync(CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.ToString("yyyyMMdd");
        var todayDate = DateTime.UtcNow.Date;
        var persistedCount = await dbContext.TaskItems.CountAsync(x => x.CreatedAt.HasValue && x.CreatedAt.Value.Date == todayDate, cancellationToken);
        var pendingCount = dbContext.ChangeTracker.Entries<TaskItem>()
            .Count(x => x.State == EntityState.Added && x.Entity.CreatedAt.HasValue && x.Entity.CreatedAt.Value.Date == todayDate);
        var count = persistedCount + pendingCount;
        return $"TASK-{today}-{count + 1:0000}";
    }
}
