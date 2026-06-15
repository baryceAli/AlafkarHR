namespace Organization.Organizations.Features.Companies.GetCompanyById;


public record GetCompanyByIdQuery(Guid Id) : IQuery<GetCompanyByIdResult>;
public record GetCompanyByIdResult(CompanyDto Company);
public class GetCompanyByIdHandler(OrganizationDbContext dbContext)
    : IQueryHandler<GetCompanyByIdQuery, GetCompanyByIdResult>
{
    public async Task<GetCompanyByIdResult> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)
    {
        var company = await dbContext.Companies
            .Include("Branches")
            .Include(x => x.ParentCompany)
            .Include(x => x.ChildCompanies)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (company is null)
            throw new NotFoundException($"Company not found: {request.Id}");

        var companyDto = company.Adapt<CompanyDto>();
        companyDto.ParentCompanyName = company.ParentCompany?.Name;
        companyDto.ChildCompaniesCount = company.ChildCompanies.Count;

        return new GetCompanyByIdResult(companyDto);
    }
}
