namespace Organization.Organizations.Features.Companies.GetCompanyById;


public record GetCompanyByIdQuery(Guid Id) : IQuery<GetCompanyByIdResult>;
public record GetCompanyByIdResult(CompanyDto Company);
public class GetCompanyByIdHandler(OrganizationDbContext dbContext)
    : IQueryHandler<GetCompanyByIdQuery, GetCompanyByIdResult>
{
    public async Task<GetCompanyByIdResult> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)
    {
        var company = await dbContext.Companies.Include("Branches").FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (company is null)
            throw new NotFoundException($"Company not found: {request.Id}");

        return new GetCompanyByIdResult(company.Adapt<CompanyDto>());
    }
}
