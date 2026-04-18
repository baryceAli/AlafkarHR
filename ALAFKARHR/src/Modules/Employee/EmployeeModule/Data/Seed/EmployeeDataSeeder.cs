using Shared.Data.Seed;

namespace EmployeeModule.Data.Seed;

public class EmployeeDataSeeder : IDataSeeder<EmployeeDbContext>
{
    public async Task SeedAllAsync(EmployeeDbContext context)
    {
        if(!await context.Positions.AnyAsync())
        {
            await context.Positions.AddAsync(InitialData.Position);
        }

    }
}
