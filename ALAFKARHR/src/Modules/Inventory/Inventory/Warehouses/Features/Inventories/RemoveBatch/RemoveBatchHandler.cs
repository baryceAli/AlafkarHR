using Inventory.Warehouses.Enums;

namespace Inventory.Warehouses.Features.Inventories.RemoveBatch;

public record RemoveInventoryBatchCommand(RemoveBatchStockDto RemoveBatch) : ICommand<RemoveInventoryBatchResult>;
public record RemoveInventoryBatchResult(bool IsSuccess);

public class RemoveInventoryBatchHandler(InventoryDbContext dbContext, IHttpContextAccessor httpContextAccessor) : ICommandHandler<RemoveInventoryBatchCommand, RemoveInventoryBatchResult>
{
    public async Task<RemoveInventoryBatchResult> Handle(RemoveInventoryBatchCommand request, CancellationToken cancellationToken)
    {
        var inventory = await dbContext.Inventories.Include("_batches")
            .FirstOrDefaultAsync(i => i.Id == request.RemoveBatch.InventoryId, cancellationToken);

        if (inventory is null)
            throw new Exception($"Inventory not found: {request.RemoveBatch.InventoryId}");

        var userId = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var batch = inventory.FindBatch(request.RemoveBatch.BatchId);

        inventory.RemoveBatch(request.RemoveBatch.BatchId, userId);
        //movement
        var movement = StockMovement.Create(Guid.NewGuid(),
            inventory.WarehouseId,
            request.RemoveBatch.BatchId,
            inventory.ProductId,
            inventory.ProductSkuId,
            batch.Quantity,
            inventory.Id,
            DateTime.UtcNow,
            MovementType.REMOVAL,
            MovementDirection.OUT,
            MovementCategory.Adjustment,
            userId,
            request.RemoveBatch.Notes
            );
        await dbContext.StockMovements.AddAsync(movement);
        //snapshot
        var snapshot = await dbContext.InventorySnapshots
            .FirstOrDefaultAsync(s =>
            s.WarehouseId == inventory.WarehouseId &&
            s.ProductSkuId == inventory.ProductSkuId && s.BatchId == request.RemoveBatch.BatchId);

        if (snapshot is not null)
        {
            snapshot.Remove(userId);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new RemoveInventoryBatchResult(true);
    }
}
