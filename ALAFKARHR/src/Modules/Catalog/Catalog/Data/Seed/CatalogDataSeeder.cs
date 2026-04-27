
using Catalog.Data.Seed;
using Microsoft.Data.SqlClient;
using Shared.SaveImages;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

public class CatalogDataSeeder : IDataSeeder<CatalogDbContext>
{
    public async Task SeedAllAsync(CatalogDbContext dbContext)
    {
        //bool retry = false;

        if (!await dbContext.Categories.AnyAsync())
        {
            await dbContext.Categories.AddRangeAsync(InitialData.Categories);
            await dbContext.SaveChangesAsync();
        }



        if (!await dbContext.Categories.AnyAsync())
        {
            await dbContext.Categories.AddRangeAsync(InitialData.Categories);
            await dbContext.SaveChangesAsync();
        }



        if (!await dbContext.Brands.AnyAsync())
        {
            await dbContext.Brands.AddRangeAsync(InitialData.Brands);
            await dbContext.SaveChangesAsync();
        }

        if (!await dbContext.Units.AnyAsync())
        {
            await dbContext.Units.AddRangeAsync(InitialData.Units);
            await dbContext.SaveChangesAsync();
        }

        if (!await dbContext.Products.AnyAsync())
        {
            foreach (var prod in InitialData.Products)
            {
                var productId = prod.Id;
                //string[] PATH_SEGEMNT = ["wwwroot", "Images", "Products"];
                //var img = SaveImages.SaveBase64Image($"{productId}", PATH_SEGEMNT, prod.ImageUrl);
                var p = Product.Create(
                    productId, 
                    prod.Name, 
                    prod.NameEng, 
                    prod.CategoryId, 
                    prod.UnitId, 
                    Guid.Parse("4C3D205F-7E2B-42C2-A081-1700B229D91E"), 
                    prod.CreatedBy);
                dbContext.Products.Add(p);
                await dbContext.SaveChangesAsync();
            }
        }

        if (!await dbContext.ProductPackages.AnyAsync())
        {
            await dbContext.ProductPackages.AddRangeAsync(InitialData.ProductPackages);
            await dbContext.SaveChangesAsync();
        }

        if (!await dbContext.Variants.AnyAsync())
        {
            await dbContext.Variants.AddRangeAsync(InitialData.Variants);
            await dbContext.SaveChangesAsync();
        }

        if (!await dbContext.ProductSkus.AnyAsync())
        {
            await dbContext.ProductSkus.AddRangeAsync(InitialData.ProductSkus);
            await dbContext.SaveChangesAsync();
        }
    }
}
