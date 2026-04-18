using Shared.Contracts.CQRS;
using Shared.Pagination;

namespace EmployeeModule.Employees.Features.AcademicInstitutions.GetAcademicInstitutions;

public record GetAcademicInstitutionsQuery(Guid CompanyId, PaginationRequest PaginationRequest) : IQuery<GetAcademicInstitutionsResult>;
public record GetAcademicInstitutionsResult(PaginatedResult<AcademicInstitutionDto> AcademicInstitutionList);
public class GetAcademicInstitutionsHandler(EmployeeDbContext dbContext)
    : IQueryHandler<GetAcademicInstitutionsQuery, GetAcademicInstitutionsResult>
{
    public async Task<GetAcademicInstitutionsResult> Handle(GetAcademicInstitutionsQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.AcademicInstitutions.AsQueryable();
        query=query.Where(a=> a.CompanyId==request.CompanyId);

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

        return new GetAcademicInstitutionsResult(
                        new PaginatedResult<AcademicInstitutionDto>(
                            request.PaginationRequest.PageIndex,
                            request.PaginationRequest.PageSize, 
                            count,
                            academics.Adapt<List<AcademicInstitutionDto>>()
                            )
                        );
    }
}
