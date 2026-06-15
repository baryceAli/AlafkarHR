using Auth.Contracts.Features.GetCompanyAdmin;

namespace Organization.Organizations.Features.Companies.GetChildCompanies;

public record GetChildCompaniesQuery(PaginationRequest PaginationRequest) : IQuery<GetChildCompaniesResult>;
public record GetChildCompaniesResult(PaginatedResult<CompanyDto> CompanyList);

public class GetChildCompaniesHandler(OrganizationDbContext dbContext, ICompanyHierarchyContext companyHierarchyContext, ISender sender)
    : IQueryHandler<GetChildCompaniesQuery, GetChildCompaniesResult>
{
    public async Task<GetChildCompaniesResult> Handle(GetChildCompaniesQuery request, CancellationToken cancellationToken)
    {
        var parentCompanyId = await companyHierarchyContext.GetCurrentParentCompanyIdAsync(cancellationToken);

        var query = dbContext.Companies
            .AsNoTracking()
            .Where(x => x.ParentCompanyId == parentCompanyId);

        if (!string.IsNullOrWhiteSpace(request.PaginationRequest.SearchText))
        {
            var search = request.PaginationRequest.SearchText.Trim();
            query = query.Where(x =>
                x.Name.Contains(search) ||
                x.NameEng.Contains(search) ||
                x.Code.Contains(search) ||
                x.VatNo.Contains(search) ||
                x.Email.Contains(search) ||
                x.Phone.Contains(search));
        }

        var count = await query.LongCountAsync(cancellationToken);
        var companies = await query
            .OrderBy(x => x.Name)
            .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
            .Take(request.PaginationRequest.PageSize)
            .ToListAsync(cancellationToken);

        var dtos = companies.Adapt<List<CompanyDto>>();
        for (var i = 0; i < companies.Count; i++)
        {
            try
            {
                var admin = await sender.Send(new GetCompanyAdminQuery(companies[i].Id), cancellationToken);
                dtos[i].AdminUserName = admin.UserName;
                dtos[i].AdminEmail = admin.Email;
                dtos[i].AdminPhoneNumber = admin.PhoneNumber;
            }
            catch
            {
                dtos[i].AdminUserName = string.Empty;
            }
        }

        return new GetChildCompaniesResult(new PaginatedResult<CompanyDto>(
            request.PaginationRequest.PageIndex,
            request.PaginationRequest.PageSize,
            count,
            dtos));
    }
}
