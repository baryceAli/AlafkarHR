using Microsoft.EntityFrameworkCore;
using Shared.Data.Seed;

namespace Pricing.Data.Seed;

public class PricingDataSeeder : IDataSeeder<PricingDbContext>
{
    public Task SeedAllAsync(PricingDbContext context)
    {
throw new NotImplementedException();
        //if (!await dbContext.Categories.AnyAsync())
        //{
        //    await dbContext.Categories.AddRangeAsync(InitialData.Categories);
        //    await dbContext.SaveChangesAsync();
        //}


    }
}
