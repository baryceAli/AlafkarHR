using Inventory.Warehouses.Enums;

namespace Inventory.Warehouses.Features.Inventories.ReleaseReservedQuantity;

public record ReleaseReservedQuantityCommand(ReleaseQuantityDto ReleaseQuantity) : ICommand<ReleaseReservedQuantityResult>;
public record ReleaseReservedQuantityResult(bool IsSuccess);
public class ReleaseReservedQuantityHandler(InventoryDbContext dbContext, IHttpContextAccessor httpContextAccessor) 
    : ICommandHandler<ReleaseReservedQuantityCommand, ReleaseReservedQuantityResult>
{
    public async Task<ReleaseReservedQuantityResult> Handle(ReleaseReservedQuantityCommand request, CancellationToken cancellationToken)
    {
        var inventory = await dbContext.Inventories.Include("_batches")
            .FirstOrDefaultAsync(x => 
                x.Id == request.ReleaseQuantity.InventoryId
                );

        if (inventory == null)
            throw new Exception($"Couldn't ind inventory: {request.ReleaseQuantity.InventoryId}");

        var userId = httpContextAccessor.HttpContext?
          .User?
          .FindFirst(ClaimTypes.NameIdentifier)?
          .Value
          ?? throw new UnauthorizedAccessException("User not authenticated");
        
        var batch= inventory.FindBatch(request.ReleaseQuantity.BatchId);
        inventory.Release(request.ReleaseQuantity.BatchId, request.ReleaseQuantity.quantity, userId);

        // movement
        var movement = StockMovement.Create(Guid.NewGuid(), 
                inventory.WarehouseId, 
                request.ReleaseQuantity.BatchId,
                inventory.ProductId,
                inventory.ProductSkuId, 
                request.ReleaseQuantity.quantity,
                inventory.Id,
                DateTime.UtcNow,
                MovementType.RELEASE, 
                MovementDirection.NONE,
                MovementCategory.Reservation,
                userId, 
                "Released quantity");
        await dbContext.AddAsync(movement);

        //snapshot

        var snapshot=await dbContext.InventorySnapshots
            .FirstOrDefaultAsync(x=> 
                x.BatchId == request.ReleaseQuantity.BatchId
                && x.WarehouseId==request.ReleaseQuantity.WarehouseId
                && x.ProductSkuId==inventory.ProductSkuId);

        if (snapshot == null)
            throw new InvalidOperationException("Snapshot not found");

        snapshot.Release(request.ReleaseQuantity.quantity, userId); 


        await dbContext.SaveChangesAsync(cancellationToken);


        return new ReleaseReservedQuantityResult(true);
    }
}
