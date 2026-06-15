using Organization.Contracts.Companies.Features.GetCompanyAccessStatus;

namespace Organization.Organizations.Features.Companies.GetCompanyAccessStatus;

public class GetCompanyAccessStatusHandler(OrganizationDbContext dbContext)
    : IQueryHandler<GetCompanyAccessStatusQuery, GetCompanyAccessStatusResult>
{
    public async Task<GetCompanyAccessStatusResult> Handle(GetCompanyAccessStatusQuery request, CancellationToken cancellationToken)
    {
        var canLogin = await dbContext.Companies
            .AsNoTracking()
            .AnyAsync(x => x.Id == request.CompanyId && x.IsActive, cancellationToken);

        return new GetCompanyAccessStatusResult(canLogin);
    }
}
