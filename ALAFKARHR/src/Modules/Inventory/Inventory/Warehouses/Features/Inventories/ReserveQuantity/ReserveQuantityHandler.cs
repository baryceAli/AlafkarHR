
//using Inventory.Warehouses.Features.Inventories.ReleaseReservedQuantity;

using Inventory.Warehouses.Enums;

namespace Inventory.Warehouses.Features.Inventories.ReserveQuantity;

public record ReserveQuantityCommand(ReserveQuantityDto ReserveQuantity) : ICommand<ReserveQuantityResult>;
public record ReserveQuantityResult(bool IsSuccess);
public class ReserveQuantityHandler(InventoryDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<ReserveQuantityCommand, ReserveQuantityResult>
{
    public async Task<ReserveQuantityResult> Handle(ReserveQuantityCommand request, CancellationToken cancellationToken)
    {
        //get the inventory with batches
        var inventory = await dbContext.Inventories.Include("_batches")
            .FirstOrDefaultAsync(x =>
                x.Id == request.ReserveQuantity.InventoryId
                );

        if (inventory == null)
            throw new Exception($"Couldn't ind inventory: {request.ReserveQuantity.InventoryId}");

        var userId = httpContextAccessor.HttpContext?
          .User?
          .FindFirst(ClaimTypes.NameIdentifier)?
          .Value
          ?? throw new UnauthorizedAccessException("User not authenticated");
        inventory.Reserve(request.ReserveQuantity.batchId,request.ReserveQuantity.quantity,userId);

        //var batch = inventory.FindBatch(request.ReserveQuantity.BatchId);
        //inventory.Release(request.ReserveQuantity., request.ReserveQuantity.quantity, userId);

        // movement
        var movement = StockMovement.Create(Guid.NewGuid(),
                inventory.WarehouseId,
                request.ReserveQuantity.batchId,
                inventory.ProductId,
                inventory.ProductSkuId,
                request.ReserveQuantity.quantity,
                inventory.Id,
                DateTime.UtcNow,
                MovementType.RELEASE,
                MovementDirection.NONE,
                MovementCategory.Reservation,
                userId,
                "Reserve quantity");
        await dbContext.AddAsync(movement);

        //snapshot

        var snapshot = await dbContext.InventorySnapshots
            .FirstOrDefaultAsync(x =>
                x.BatchId == request.ReserveQuantity.batchId
                && x.WarehouseId == request.ReserveQuantity.WarehouseId
                && x.ProductSkuId == inventory.ProductSkuId);

        if (snapshot == null)
            throw new InvalidOperationException("Snapshot not found");

        snapshot.Release(request.ReserveQuantity.quantity, userId);


        await dbContext.SaveChangesAsync(cancellationToken);


        return new ReserveQuantityResult(true);
    }
}
