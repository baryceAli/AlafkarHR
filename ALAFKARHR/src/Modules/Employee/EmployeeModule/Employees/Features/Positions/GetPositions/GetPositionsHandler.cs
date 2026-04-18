using Shared.Contracts.CQRS;
using Shared.Pagination;

namespace EmployeeModule.Employees.Features.Positions.GetPositions;

public record GetPositionsQuery(Guid CompanyId,PaginationRequest PaginationRequest) : IQuery<GetPositionsResult>;
public record GetPositionsResult(PaginatedResult<PositionDto> PositionList);
public class GetPositionsHandler(EmployeeDbContext dbContext) : IQueryHandler<GetPositionsQuery, GetPositionsResult>
{
    public async Task<GetPositionsResult> Handle(GetPositionsQuery request, CancellationToken cancellationToken)
    {
        var qurery = dbContext.Positions.AsNoTracking().AsQueryable();

        qurery = qurery.Where(p => p.CompanyId == request.CompanyId && p.IsDeleted==false);

        if (!string.IsNullOrWhiteSpace(request.PaginationRequest.SearchText))
        {
            var search =request.PaginationRequest.SearchText.ToLower();

            qurery = qurery.Where(p => p.Title.ToLower().Contains(search)
                                    || p.TitleEng.ToLower().Contains(search)
                                    || p.Code.ToLower().Contains(search));
        }

        long count =await qurery.LongCountAsync(cancellationToken);

        var positions =await qurery.OrderBy(p => p.Title)
                            .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
                            .Take(request.PaginationRequest.PageSize)
                            .ToListAsync();

        return new GetPositionsResult(new PaginatedResult<PositionDto>(
                                    request.PaginationRequest.PageIndex,
                                    request.PaginationRequest.PageSize,
                                    count,
                                    positions.Adapt<List<PositionDto>>()
                                )
                    );

    }
}
