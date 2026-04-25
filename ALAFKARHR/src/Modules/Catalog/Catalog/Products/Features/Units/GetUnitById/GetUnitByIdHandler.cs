using Mapster;

namespace Catalog.Products.Features.Units.GetUnitById;

public record GetUnitByIdQuery(Guid Id) : IQuery<GetUnitByIdResult>;
public record GetUnitByIdResult(UnitDto Unit);
public class GetUnitByIdHandler (CatalogDbContext dbContext)
    : IQueryHandler<GetUnitByIdQuery, GetUnitByIdResult>
{
    public async Task<GetUnitByIdResult> Handle(GetUnitByIdQuery query, CancellationToken cancellationToken)
    {
        var unit= await dbContext.Units.AsNoTracking().FirstOrDefaultAsync(x=> x.Id==query.Id && x.DeletedAt==null);
        if (unit is null)
            throw new Exception($"Unit not found: {query.Id}");

        return new GetUnitByIdResult(unit.Adapt<UnitDto>());
    }
}
