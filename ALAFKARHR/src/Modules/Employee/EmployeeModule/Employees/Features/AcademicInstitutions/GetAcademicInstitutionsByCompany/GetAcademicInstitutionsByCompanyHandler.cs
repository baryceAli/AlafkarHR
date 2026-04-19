using Shared.Contracts.CQRS;
using Shared.Pagination;

namespace EmployeeModule.Employees.Features.AcademicInstitutions.GetAcademicInstitutionsByCompany;

public record GetAcademicInstitutionsByCompanyQuery(Guid CompanyId, PaginationRequest PaginationRequest) : IQuery<GetAcademicInstitutionsByCompanyResult>;
public record GetAcademicInstitutionsByCompanyResult(PaginatedResult<AcademicInstitutionDto> AcademicInstitutionList);
public class GetAcademicInstitutionsByCompanyHandler(EmployeeDbContext dbContext)
    : IQueryHandler<GetAcademicInstitutionsByCompanyQuery, GetAcademicInstitutionsByCompanyResult>
{
    public async Task<GetAcademicInstitutionsByCompanyResult> Handle(GetAcademicInstitutionsByCompanyQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.AcademicInstitutions.AsQueryable();
        query=query.Where(a=> a.CompanyId==request.CompanyId && a.IsDeleted==false);

        if (!string.IsNullOrWhiteSpace(request.PaginationRequest.SearchText))
        {
            var search = request.PaginationRequest.SearchText.ToLower();

            query.Where(a => a.Name.ToLower().Contains(search) ||
                            a.NameEng.ToLower().Contains(search)
            );

        }

        long count = await query.LongCountAsync(cancellationToken);
        var  academics=await query
                            .OrderBy(a => a.Name)
                            .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
                            .Take(request.PaginationRequest.PageSize)
                            .ToListAsync(cancellationToken);

        return new GetAcademicInstitutionsByCompanyResult(
                        new PaginatedResult<AcademicInstitutionDto>(
                            request.PaginationRequest.PageIndex,
                            request.PaginationRequest.PageSize, 
                            count,
                            academics.Adapt<List<AcademicInstitutionDto>>()
                            )
                        );
    }
}
