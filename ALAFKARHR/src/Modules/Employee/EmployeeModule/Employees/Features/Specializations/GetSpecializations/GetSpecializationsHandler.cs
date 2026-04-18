using Shared.Contracts.CQRS;
using Shared.Pagination;

namespace EmployeeModule.Employees.Features.Specializations.GetSpecializations;

public record GetSpecializationsQuery(Guid CompanyId, PaginationRequest PaginationRequest) : IQuery<GetSpecializationsResult>;
public record GetSpecializationsResult(PaginatedResult<SpecializationDto> SpecializationList);
public class GetSpecializationsHandler(EmployeeDbContext dbContext)
    : IQueryHandler<GetSpecializationsQuery, GetSpecializationsResult>
{
    public async Task<GetSpecializationsResult> Handle(GetSpecializationsQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Specializations.AsQueryable();
        query = query.Where(s => s.CompanyId == request.CompanyId);

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

        return new GetSpecializationsResult(
            new PaginatedResult<SpecializationDto>(
                request.PaginationRequest.PageIndex,
                request.PaginationRequest.PageSize,
                count, 
                specs.Adapt<List<SpecializationDto>>()
                )
            );
    }
}
