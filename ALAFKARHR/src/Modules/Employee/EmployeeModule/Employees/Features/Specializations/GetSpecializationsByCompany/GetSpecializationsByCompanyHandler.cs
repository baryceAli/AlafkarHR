using Shared.Contracts.CQRS;
using Shared.Pagination;

namespace EmployeeModule.Employees.Features.Specializations.GetSpecializationsByCompany;

public record GetSpecializationsByCompanyQuery(Guid CompanyId, PaginationRequest PaginationRequest) : IQuery<GetSpecializationsByCompanyResult>;
public record GetSpecializationsByCompanyResult(PaginatedResult<SpecializationDto> SpecializationList);
public class GetSpecializationsByCompanyHandler(EmployeeDbContext dbContext)
    : IQueryHandler<GetSpecializationsByCompanyQuery, GetSpecializationsByCompanyResult>
{
    public async Task<GetSpecializationsByCompanyResult> Handle(GetSpecializationsByCompanyQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Specializations.AsQueryable();
        query = query.Where(s => s.CompanyId == request.CompanyId && s.IsDeleted==false);

        if (!string.IsNullOrWhiteSpace(request.PaginationRequest.SearchText))
        {
            var search = request.PaginationRequest.SearchText.ToLower();

            query=query
                .Where(s=> 
                        s.Name.ToLower().Contains(search)||
                        s.NameEng.ToLower().Contains(search)
            );
        }
        long count = await query.LongCountAsync(cancellationToken);
        var specs = await query
                            .OrderBy(s => s.Name)
                            .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
                            .Take(request.PaginationRequest.PageSize)
                            .ToListAsync(cancellationToken);

        return new GetSpecializationsByCompanyResult(
            new PaginatedResult<SpecializationDto>(
                request.PaginationRequest.PageIndex,
                request.PaginationRequest.PageSize,
                count, 
                specs.Adapt<List<SpecializationDto>>()
                )
            );
    }
}
