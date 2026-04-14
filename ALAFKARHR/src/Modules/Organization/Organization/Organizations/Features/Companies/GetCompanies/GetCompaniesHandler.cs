namespace Organization.Organizations.Features.Companies.GetCompanies;

public record GetCompaniesQuery(PaginationRequest PaginationRequest) : IQuery<GetCompaniesResult>;
public record GetCompaniesResult(PaginatedResult<CompanyDto> CompanyList);
public class GetCompaniesHandler(OrganizationDbContext dbContext)
    : IQueryHandler<GetCompaniesQuery, GetCompaniesResult>
{
    public async Task<GetCompaniesResult> Handle(GetCompaniesQuery request, CancellationToken cancellationToken)
    {
        long count = await dbContext.Companies.LongCountAsync();
        var companies = await dbContext.Companies
            .AsNoTracking()
            .Skip(request.PaginationRequest.PageSize * request.PaginationRequest.PageIndex)
            .Take(request.PaginationRequest.PageSize)
            .Include("Branches")
            .ToListAsync();

        var companyDtos=companies.Adapt<List<CompanyDto>>();

        return new GetCompaniesResult(
            new PaginatedResult<CompanyDto>(
                request.PaginationRequest.PageIndex,
                request.PaginationRequest.PageSize, 
                count, 
                companyDtos));
    }
}
