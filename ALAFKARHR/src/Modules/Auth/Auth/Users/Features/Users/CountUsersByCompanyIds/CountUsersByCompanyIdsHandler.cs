using Auth.Contracts.Features.CountUsersByCompanyIds;

namespace Auth.Users.Features.Users.CountUsersByCompanyIds;

public class CountUsersByCompanyIdsHandler(AuthDbContext dbContext)
    : IQueryHandler<CountUsersByCompanyIdsQuery, CountUsersByCompanyIdsResult>
{
    public async Task<CountUsersByCompanyIdsResult> Handle(CountUsersByCompanyIdsQuery request, CancellationToken cancellationToken)
    {
        if (request.CompanyIds.Count == 0)
            return new CountUsersByCompanyIdsResult(0);

        var count = await dbContext.Users
            .AsNoTracking()
            .CountAsync(x => x.CompanyId.HasValue && request.CompanyIds.Contains(x.CompanyId.Value), cancellationToken);

        return new CountUsersByCompanyIdsResult(count);
    }
}
