namespace TaskManagement.Tasks.Services;

public class TaskNumberGenerator(TaskManagementDbContext dbContext) : ITaskNumberGenerator
{
    public async Task<string> GenerateAsync(CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.ToString("yyyyMMdd");
        var count = await dbContext.TaskItems.CountAsync(x => x.CreatedAt.Value.Date == DateTime.UtcNow.Date, cancellationToken);
        return $"TASK-{today}-{count + 1:0000}";
    }
}
