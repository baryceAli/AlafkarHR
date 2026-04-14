namespace Organization.Organizations.Features.Branches.GetByCompanyId;

public record GetByCompanyIdQuery(Guid companyId) : IQuery<GetByCompanyIdResult>;
public record GetByCompanyIdResult(List<BranchDto> BranchList);
public class GetByCompanyIdHandler(OrganizationDbContext dbContext)
    : IQueryHandler<GetByCompanyIdQuery, GetByCompanyIdResult>
{
    public async Task<GetByCompanyIdResult> Handle(GetByCompanyIdQuery request, CancellationToken cancellationToken)
    {
        var companies = await dbContext.Branches
            .AsNoTracking()
            .Where(x => x.CompanyId == request.companyId)
            .ToListAsync();

        return new GetByCompanyIdResult(companies.Adapt<List<BranchDto>>());

    }
}
