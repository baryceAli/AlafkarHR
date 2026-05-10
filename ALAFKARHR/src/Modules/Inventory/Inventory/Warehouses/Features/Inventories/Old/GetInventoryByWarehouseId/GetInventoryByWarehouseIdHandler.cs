namespace Inventory.Warehouses.Features.Inventories.GetInventoryByWarehouseId;


public record GetInventoryByWarehouseIdQuery(Guid WarehouseId) : IQuery<GetInventoryByWarehouseIdResult>;
public record GetInventoryByWarehouseIdResult(List<InventoryAggregateDto> InventoryList);
public class GetInventoryByWarehouseIdHandler(InventoryDbContext dbContext)
    : IQueryHandler<GetInventoryByWarehouseIdQuery, GetInventoryByWarehouseIdResult>
{
    public async Task<GetInventoryByWarehouseIdResult> Handle(GetInventoryByWarehouseIdQuery request, CancellationToken cancellationToken)
    {
        var inventories = await dbContext.Inventories.Include("Batches")
            .AsNoTracking()
            .Where(i => i.WarehouseId == request.WarehouseId)
            .ToListAsync();

        return new GetInventoryByWarehouseIdResult(inventories.Adapt<List<InventoryAggregateDto>>());
    }
}
