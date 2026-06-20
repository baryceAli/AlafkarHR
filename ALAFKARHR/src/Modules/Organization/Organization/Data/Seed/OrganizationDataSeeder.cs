using Shared.Data.Seed;
using SharedWithUI.Organization.Enums;

namespace Organization.Data.Seed;

public class OrganizationDataSeeder : IDataSeeder<OrganizationDbContext>
{
    private const string SeedActor = "local-seed";

    public async Task SeedAllAsync(OrganizationDbContext dbContext)
    {
        await EnsureLicenseCategoriesAsync(dbContext);

        if (!await dbContext.Companies.AnyAsync())
        {
            await dbContext.Companies.AddAsync(InitialData.Company);
            await dbContext.SaveChangesAsync();
        }

        if (!await dbContext.Branches.AnyAsync())
        {
            foreach (var branch in InitialData.Branches)
            {
                await dbContext.Branches.AddAsync(branch);
            }
        }

        await EnsureParentCompanyLicensesAsync(dbContext);
        await dbContext.SaveChangesAsync();
    }

    private static async Task EnsureLicenseCategoriesAsync(OrganizationDbContext dbContext)
    {
        foreach (var category in InitialData.LicenseCategories)
        {
            var existing = await dbContext.LicenseCategories
                .FirstOrDefaultAsync(item => item.Key == category.Key);

            if (existing is null)
            {
                await dbContext.LicenseCategories.AddAsync(category);
            }
        }
    }

    private static async Task EnsureParentCompanyLicensesAsync(OrganizationDbContext dbContext)
    {
        var parentCompanyIds = await dbContext.Companies
            .Where(company => company.ParentCompanyId == null)
            .Select(company => company.Id)
            .ToListAsync();

        foreach (var companyId in parentCompanyIds)
        {
            var hasLicense = await dbContext.CompanyLicenses
                .AnyAsync(license => license.CompanyId == companyId);
            if (hasLicense)
            {
                continue;
            }

            await dbContext.CompanyLicenses.AddAsync(CompanyLicense.Create(
                Guid.NewGuid(),
                companyId,
                CompanyLicenseStatus.Active,
                "standard",
                "Standard",
                DateTime.UtcNow.Date,
                DateTime.UtcNow.Date.AddYears(1),
                25,
                5,
                10,
                "Default local development license.",
                SeedActor,
                InitialData.StandardLicenseCategoryId));
        }
    }
}
