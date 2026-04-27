namespace Catalog.Products.Features.Units.GetUnits;

public record GetUnitsQuery(PaginationRequest PaginationRequest) : IQuery<GetUnitsResult>;
public record GetUnitsResult(PaginatedResult<UnitDto> UnitList);

public class GetUnitsHandler (CatalogDbContext dbContext)
    : IQueryHandler<GetUnitsQuery, GetUnitsResult>
{
    public async Task<GetUnitsResult> Handle(GetUnitsQuery request, CancellationToken cancellationToken)
    {

        var query = dbContext.Units.AsQueryable();
        query=query.Where(x => x.IsDeleted == false);


        if (!string.IsNullOrWhiteSpace(request.PaginationRequest.SearchText))
        {
            query = query.
                Where(x => 
                    x.UnitName.ToLower().Contains(request.PaginationRequest.SearchText.ToLower()) || 
                    x.UnitNameEng.ToLower().Contains(request.PaginationRequest.SearchText.ToLower()));
        }
        
        var count = await query.LongCountAsync(cancellationToken);


        var units = await query
                .AsNoTracking()
                .Where(x => x.IsDeleted==false)
                .OrderBy(x=> x.UnitName)
                .Skip(request.PaginationRequest.PageSize * request.PaginationRequest.PageIndex)
                .Take(request.PaginationRequest.PageSize)
                .ToListAsync();

        var unitsDto = units.Adapt<List<UnitDto>>();
        return new GetUnitsResult(new PaginatedResult<UnitDto>(
            request.PaginationRequest.PageIndex,
            request.PaginationRequest.PageSize,
            count,
            unitsDto
            ));

    }
}
