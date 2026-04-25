namespace Catalog.Products.Features.Units.GetUnits;

public record GetUnitsQuery(PaginationRequest PaginationRequest) : IQuery<GetUnitsResult>;
public record GetUnitsResult(PaginatedResult<UnitDto> UnitList);

public class GetUnitsHandler (CatalogDbContext dbContext)
    : IQueryHandler<GetUnitsQuery, GetUnitsResult>
{
    public async Task<GetUnitsResult> Handle(GetUnitsQuery request, CancellationToken cancellationToken)
    {
        var pageIndex= request.PaginationRequest.PageIndex;
        var pageSize= request.PaginationRequest.PageSize;
        var count = await dbContext.Units.LongCountAsync(cancellationToken);

        var units = await dbContext.Units
                .AsNoTracking()
                .Where(x => x.DeletedAt == null)
                .OrderBy(x=> x.UnitName)
                .Skip(pageIndex)
                .Take(pageSize)
                .ToListAsync();

        var unts = units.Adapt<List<UnitDto>>();
        return new GetUnitsResult(new PaginatedResult<UnitDto>(
            pageIndex,
            pageSize,
            count,
            unts
            ));

    }
}
