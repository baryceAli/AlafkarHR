using Microsoft.EntityFrameworkCore;
using Shared.Data.Seed;

namespace GeneralSettings.Data.Seed;

public class GeneralSettingsDataSeeder : IDataSeeder<GeneralSettingsDbContext>
{
    public async Task SeedAllAsync(GeneralSettingsDbContext context)
    {
        if (!await context.Currencies.AnyAsync())
        {
            foreach (var currency in InitialData.currencies)
            {
                await context.Currencies.AddAsync(currency);
                await context.SaveChangesAsync();
            }
        }

        if (!await context.CompanySettings.AnyAsync())
        {
            foreach (var companySetting in InitialData.companySettings)
            {
                await context.CompanySettings.AddAsync(companySetting);
                await context.SaveChangesAsync();
            }
        }
    }
}
