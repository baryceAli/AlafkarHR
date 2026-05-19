using Microsoft.EntityFrameworkCore;
using Shared.Data.Seed;

namespace CustomersModule.Data.Seed;

public class CustomerDataSeeder : IDataSeeder<CustomerDbContext>
{
    public async Task SeedAllAsync(CustomerDbContext context)
    {
        if (!await context.CustomerGroups.AnyAsync())
        {
            foreach (var group in InitialData.CustomerGroups)
            {
                await context.CustomerGroups.AddAsync(group);
                await context.SaveChangesAsync();
            }
        }
    }
}
