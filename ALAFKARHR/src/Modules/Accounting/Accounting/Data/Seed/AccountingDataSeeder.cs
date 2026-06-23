using Shared.Data.Seed;

namespace Accounting.Data.Seed;

public class AccountingDataSeeder(ISender sender) : IDataSeeder<AccountingDbContext>
{
    public async Task SeedAllAsync(AccountingDbContext context)
    {
        var companyIds = await context.Accounts.AsNoTracking()
            .Where(x => x.ParentAccountId == null && !x.IsPostingAccount && x.IsActive)
            .Select(x => x.CompanyId)
            .Distinct()
            .ToListAsync();

        foreach (var companyId in companyIds)
        {
            var branches = await sender.Send(new GetCompanyBranchesForAccountingQuery(companyId));
            foreach (var branch in branches.Branches)
            {
                await sender.Send(new EnsureBranchAccountingCommand(
                    branch.CompanyId,
                    branch.BranchId,
                    branch.Code,
                    branch.Name,
                    branch.NameEng));
            }
        }
    }
}
