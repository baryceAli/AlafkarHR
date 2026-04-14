using Shared.Data.Seed;

namespace Organization.Data.Seed;

public class OrganizationDataSeeder : IDataSeeder<OrganizationDbContext>
{
    public async Task SeedAllAsync(OrganizationDbContext dbContext)
    {
        if (!await dbContext.Companies.AnyAsync())
        {

            await dbContext.Companies.AddAsync(InitialData.Company);
            await dbContext.SaveChangesAsync();
        }
        if(!await dbContext.Branches.AnyAsync())
        {
            foreach(var br in InitialData.Branches)
            {
                await dbContext.Branches.AddAsync(br);
            }
        }


    }
}
