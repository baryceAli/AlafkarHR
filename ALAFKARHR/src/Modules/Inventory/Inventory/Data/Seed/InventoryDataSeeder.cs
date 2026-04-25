using Shared.Data.Seed;

namespace Inventory.Data.Seed;

public class InventoryDataSeeder : IDataSeeder<InventoryDbContext>
{
    public async Task SeedAllAsync(InventoryDbContext dbContext)
    {
        //return;
        if (!await dbContext.Warehouses.AnyAsync())
        {
            await dbContext.Warehouses.AddRangeAsync(InitialData.Warehouses);
            await dbContext.SaveChangesAsync();
        }

        //if(!await dbContext.InventoryItems.AnyAsync())
        //{
        //    await dbContext.InventoryItems.AddRangeAsync(InitialData.InventoryItems);
        //    await dbContext.SaveChangesAsync();
        //}
    }
}
