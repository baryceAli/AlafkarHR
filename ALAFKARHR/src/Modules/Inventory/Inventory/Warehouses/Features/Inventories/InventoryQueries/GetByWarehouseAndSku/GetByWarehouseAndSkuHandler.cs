using Shared.Exceptions;

namespace Inventory.Warehouses.Features.Inventories.InventoryQueries.GetByWarehouseAndSku;


public record GetByWarehouseAndSkuQuery(Guid WarehouseId, Guid SkuId) : IQuery<GetByWarehouseAndSkuResult>;
public record GetByWarehouseAndSkuResult(InventoryAggregateDto InventoryAggregate);
public class GetByWarehouseAndSkuHandler(InventoryDbContext dbContext, ISender sender)
    : IQueryHandler<GetByWarehouseAndSkuQuery, GetByWarehouseAndSkuResult>
{
    public async Task<GetByWarehouseAndSkuResult> Handle(GetByWarehouseAndSkuQuery request, CancellationToken cancellationToken)
    {
        var inventory = await dbContext.Inventories.Include(i => i.Batches).AsNoTracking().FirstOrDefaultAsync(i => i.WarehouseId == request.WarehouseId && i.ProductSkuId == request.SkuId, cancellationToken);
        if (inventory is null)
            throw new NotFoundException($"Inventory not found for sku ({request.SkuId}) in warehouse ({request.WarehouseId})");
        await global::Inventory.Warehouses.Features.Inventories.InventoryBranchScope.EnsureCanReadWarehouseAsync(
            dbContext,
            sender,
            inventory.CompanyId,
            request.WarehouseId,
            cancellationToken);
        //if(inventory.TotalQuantity==0)
        return new GetByWarehouseAndSkuResult(inventory.Adapt<InventoryAggregateDto>());
    }
}
