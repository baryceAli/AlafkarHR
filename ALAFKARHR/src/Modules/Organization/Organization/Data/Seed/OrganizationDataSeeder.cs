using Shared.Data.Seed;
using SharedWithUI.Organization.Enums;

namespace Organization.Data.Seed;

public class OrganizationDataSeeder : IDataSeeder<OrganizationDbContext>
{
    private const string SeedActor = "local-seed";

    public async Task SeedAllAsync(OrganizationDbContext dbContext)
    {
        await EnsureBusinessLinesAsync(dbContext);
        await EnsureLicenseCategoriesAsync(dbContext);
        await EnsureLicenseCategoryBusinessLinesAsync(dbContext);

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

        await EnsureMainBranchesAsync(dbContext);
        await EnsureParentCompanyLicensesAsync(dbContext);
        await EnsureCompanyLicenseBusinessLinesAsync(dbContext);
        await dbContext.SaveChangesAsync();
    }

    private static async Task EnsureBusinessLinesAsync(OrganizationDbContext dbContext)
    {
        foreach (var businessLine in InitialData.BusinessLines)
        {
            var existing = await dbContext.BusinessLines
                .FirstOrDefaultAsync(item => item.Key == businessLine.Key);

            if (existing is null)
            {
                await dbContext.BusinessLines.AddAsync(businessLine);
            }
        }

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

    private static async Task EnsureLicenseCategoryBusinessLinesAsync(OrganizationDbContext dbContext)
    {
        var categories = await dbContext.LicenseCategories.ToListAsync();
        foreach (var category in categories)
        {
            foreach (var businessLineId in InitialData.ImplementedBusinessLineIds)
            {
                var exists = await dbContext.LicenseCategoryBusinessLines
                    .AnyAsync(item => item.LicenseCategoryId == category.Id && item.BusinessLineId == businessLineId);

                if (!exists)
                {
                    await dbContext.LicenseCategoryBusinessLines.AddAsync(
                        LicenseCategoryBusinessLine.Create(category.Id, businessLineId, DefaultActivationLimit(businessLineId)));
                }
            }
        }

        await dbContext.SaveChangesAsync();
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

    private static async Task EnsureMainBranchesAsync(OrganizationDbContext dbContext)
    {
        var companies = await dbContext.Companies.ToListAsync();

        foreach (var company in companies)
        {
            var hasMainBranch = await dbContext.Branches
                .AnyAsync(branch => branch.CompanyId == company.Id && branch.IsMainBranch);
            if (hasMainBranch)
            {
                continue;
            }

            var existingMainCodeBranch = await dbContext.Branches
                .FirstOrDefaultAsync(branch => branch.CompanyId == company.Id && branch.Code == "MAIN");
            if (existingMainCodeBranch is not null)
            {
                existingMainCodeBranch.Update(
                    existingMainCodeBranch.Name,
                    existingMainCodeBranch.NameEng,
                    existingMainCodeBranch.Location,
                    existingMainCodeBranch.Longitude,
                    existingMainCodeBranch.Latitude,
                    existingMainCodeBranch.Code,
                    existingMainCodeBranch.Phone,
                    existingMainCodeBranch.Email,
                    true,
                    SeedActor);
                continue;
            }

            await dbContext.Branches.AddAsync(Branch.Create(
                Guid.NewGuid(),
                company.Name,
                company.NameEng,
                company.HqLocation,
                company.HqLongitude,
                company.HqLatitude,
                "MAIN",
                company.Phone,
                company.Email,
                true,
                company.Id,
                SeedActor));
        }
    }

    private static async Task EnsureCompanyLicenseBusinessLinesAsync(OrganizationDbContext dbContext)
    {
        var licenses = await dbContext.CompanyLicenses.ToListAsync();
        foreach (var license in licenses)
        {
            foreach (var businessLineId in InitialData.ImplementedBusinessLineIds)
            {
                var exists = await dbContext.CompanyLicenseBusinessLines
                    .AnyAsync(item => item.CompanyLicenseId == license.Id && item.BusinessLineId == businessLineId);

                if (!exists)
                {
                    await dbContext.CompanyLicenseBusinessLines.AddAsync(
                        CompanyLicenseBusinessLine.Create(license.Id, businessLineId, DefaultActivationLimit(businessLineId)));
                }
            }
        }
    }

    private static int DefaultActivationLimit(Guid businessLineId) =>
        businessLineId == InitialData.StoreFrontBusinessLineId ? 3 : 1;
}
