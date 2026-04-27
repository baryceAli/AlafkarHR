using Catalog.Products.Features.Units.GetUnits;

namespace Catalog.Products.Features.Units.GetUnitsByComapny;


public record GetUnitsByComapnyQuery(Guid CompanyId,PaginationRequest PaginationRequest) : IQuery<GetUnitsByComapnyResult>;
public record GetUnitsByComapnyResult(PaginatedResult<UnitDto> UnitList);
public class GetUnitsByComapnyHandler(CatalogDbContext dbContext)
    : IQueryHandler<GetUnitsByComapnyQuery, GetUnitsByComapnyResult>
{
    public async Task<GetUnitsByComapnyResult> Handle(GetUnitsByComapnyQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Units.AsQueryable();
        query = query.Where(x => x.CompanyId==request.CompanyId && x.IsDeleted == false);


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
                .Where(x => x.IsDeleted == false)
                .OrderBy(x => x.UnitName)
                .Skip(request.PaginationRequest.PageSize * request.PaginationRequest.PageIndex)
                .Take(request.PaginationRequest.PageSize)
                .ToListAsync();

        var unitsDto = units.Adapt<List<UnitDto>>();
        return new GetUnitsByComapnyResult(new PaginatedResult<UnitDto>(
            request.PaginationRequest.PageIndex,
            request.PaginationRequest.PageSize,
            count,
            unitsDto
            ));
    }
}
