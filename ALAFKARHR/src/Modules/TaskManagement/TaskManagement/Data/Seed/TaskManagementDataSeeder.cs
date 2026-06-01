namespace TaskManagement.Data.Seed;

public class TaskManagementDataSeeder : IDataSeeder<TaskManagementDbContext>
{
    public async Task SeedAllAsync(TaskManagementDbContext dbContext)
    {
        await dbContext.SaveChangesAsync();
    }
}
