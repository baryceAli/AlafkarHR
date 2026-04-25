using Inventory.Warehouses.Enums;

namespace Inventory.Warehouses.Features.Inventories.CreateInventory;

public record CreateInventoryCommand(CreateInventoryAggregateDto Inventory) : ICommand<CreateInventoryResult>;
public record CreateInventoryResult(Guid Id);
public class CreateInventoryCommandValidator : AbstractValidator<CreateInventoryCommand>
{
    public CreateInventoryCommandValidator()
    {
        RuleFor(x => x.Inventory.WarehouseId).NotNull().WithMessage("WarehouseId is required");
        RuleFor(x => x.Inventory.ProductId).NotNull().WithMessage("ProductId is required");
        RuleFor(x => x.Inventory.ProductSkuId).NotNull().WithMessage("ProductSkuId is required");
        RuleFor(x => x.Inventory.InitialBatchId).NotNull().WithMessage("InitialBatchId is required");
        RuleFor(x => x.Inventory.InitialQuantity).GreaterThan(0).WithMessage("InitialQuantity must be greator than 0");
    }
}
public class CreateInventoryHandler(InventoryDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CreateInventoryCommand, CreateInventoryResult>
{
    public async Task<CreateInventoryResult> Handle(CreateInventoryCommand request, CancellationToken cancellationToken)
    {
        var exist = await dbContext.Inventories
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.ProductSkuId == request.Inventory.ProductSkuId
                && x.WarehouseId == request.Inventory.WarehouseId);

        if (exist != null)
            throw new Exception($"Inventory already exists for {request.Inventory.ProductSkuId} and {request.Inventory.WarehouseId}");

        var user = httpContextAccessor.HttpContext.User;
        var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;


        var inventory = InventoryAggregate.Create(Guid.NewGuid()
            , request.Inventory.ProductId,
            request.Inventory.ProductSkuId,
            request.Inventory.WarehouseId, userId);
        // Create batch
        var batch = BatchStock.Created(
            Guid.NewGuid(),
            inventory.WarehouseId,
            request.Inventory.InitialBatchId,
            userId
            );
        inventory.AddBatch(batch);
        // Apply stock to aggregate
        inventory.TransferIn(batch.BatchId, request.Inventory.InitialQuantity, userId);

        var stockmovement = StockMovement.Create(Guid.NewGuid(),
            request.Inventory.WarehouseId,
            request.Inventory.InitialBatchId,
            request.Inventory.ProductId,
            request.Inventory.ProductSkuId,
            request.Inventory.InitialQuantity, 
            inventory.Id, 
            DateTime.UtcNow,
            MovementType.OPENING,
            MovementDirection.IN,
            MovementCategory.Physical,
            userId, 
            "New Inventory Created");

        var inventorySnapshot = InventorySnapshot.Create(Guid.NewGuid(),
            request.Inventory.ProductId,
            request.Inventory.ProductSkuId,
            request.Inventory.WarehouseId,
            request.Inventory.InitialBatchId,
            request.Inventory.InitialQuantity,
            0,
            userId);

        await dbContext.Inventories.AddAsync(inventory, cancellationToken);
        await dbContext.StockMovements.AddAsync(stockmovement, cancellationToken);
        await dbContext.InventorySnapshots.AddAsync(inventorySnapshot, cancellationToken);

        await dbContext.SaveChangesAsync();
        return new CreateInventoryResult(inventory.Id);
    }
}
