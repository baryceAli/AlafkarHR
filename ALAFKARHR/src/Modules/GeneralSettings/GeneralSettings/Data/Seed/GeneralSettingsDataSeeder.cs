using Microsoft.EntityFrameworkCore;
using Shared.Data.Seed;

namespace GeneralSettings.Data.Seed;

public class GeneralSettingsDataSeeder : IDataSeeder<GeneralSettingsDbContext>
{
    public  async Task SeedAllAsync(GeneralSettingsDbContext context)
    {
       await Task.CompletedTask;
        //foreach (var currency in InitialData.currencies)
        //{
        //    var code = currency.Code.Trim().ToUpperInvariant();
        //    var exists = await context.Currencies.AnyAsync(existing =>
        //        existing.CompanyId == currency.CompanyId &&
        //        !existing.IsDeleted &&
        //        existing.Code.ToUpper() == code);

        //    if (!exists)
        //    {
        //        await context.Currencies.AddAsync(currency);
        //    }
        //}

        //await context.SaveChangesAsync();

        //if (!await context.CompanySettings.AnyAsync())
        //{
        //    foreach (var companySetting in InitialData.companySettings)
        //    {
        //        await context.CompanySettings.AddAsync(companySetting);
        //        await context.SaveChangesAsync();
        //    }
        //}
    }
}
