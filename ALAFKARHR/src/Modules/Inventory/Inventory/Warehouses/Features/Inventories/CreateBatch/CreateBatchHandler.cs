using Inventory.Warehouses.Enums;

namespace Inventory.Warehouses.Features.Inventories.Create;

public record CreateBatchCommand(CreateBatchDto Batch) : ICommand<CreateBatchResult>;
public record CreateBatchResult(bool IsSuccess);

public class CreateBatchCommandValidator : AbstractValidator<CreateBatchCommand>
{
    public CreateBatchCommandValidator()
    {
        RuleFor(x => x.Batch).NotNull().WithMessage("Batch is required");
        //RuleFor(x => x.Batch.WarehouseId).NotNull().WithMessage("WarehouseId is required");
        RuleFor(x => x.Batch.ProductId).NotNull().WithMessage("ProductId is required");
        RuleFor(x => x.Batch.ProductSkuId).NotNull().WithMessage("ProductSkuId is required");
        RuleFor(x => x.Batch.BatchNumber).NotEmpty().WithMessage("BatchNumber is required");
        RuleFor(x => x.Batch.ManufacturingDate).NotNull().WithMessage("ManufacturingDate is required");
        RuleFor(x => x.Batch.ExpiryDate).NotNull().WithMessage("ExpiryDate is required");
    }
}

public class CreateBatchHandler(InventoryDbContext dbContext, IHttpContextAccessor httpContextAccessor) : ICommandHandler<CreateBatchCommand, CreateBatchResult>
{
    public async Task<CreateBatchResult> Handle(CreateBatchCommand command, CancellationToken cancellationToken)
    {
        //var ExistInventory=await dbContext.Batches
        //    .FirstOrDefaultAsync(i =>
        //            //i.WarehouseId == command.Batch.WarehouseId
        //             i.ProductId == command.Batch.ProductId
        //            && i.ProductSkuId == command.Batch.ProductSkuId
        //            , cancellationToken);
        //if (ExistInventory is not null)
        //    throw new Exception($"Inventory found for  Product ({command.Batch.ProductId}) and ProductSKU ({command.Batch.ProductSkuId})");

        var existsBatch = await dbContext.Batches
            .FirstOrDefaultAsync(i =>
                    i.BatchNumber==command.Batch.BatchNumber
                    //i.ProductId == command.Batch.ProductId
                    //&& i.ProductSkuId == command.Batch.ProductSkuId
                    , cancellationToken);

        if (existsBatch is not null)
            throw new Exception($"Batch found for  product({command.Batch.ProductId})");
        existsBatch = await dbContext.Batches
            .FirstOrDefaultAsync(i =>
                    i.BatchNumber==command.Batch.BatchNumber, cancellationToken);

        if (existsBatch is not null)
            throw new Exception($"BatchNumber is douplication ({command.Batch.BatchNumber})");

        var userId = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var batch = Batch.Create(Guid.NewGuid(), 
            //command.Batch.WarehouseId,
            command.Batch.ProductId,
            command.Batch.ProductSkuId,
            command.Batch.BatchNumber,
            command.Batch.ManufacturingDate,
            command.Batch.ExpiryDate, 
            userId);

        await dbContext.Batches.AddAsync(batch);

        
         //var inventory = InventoryAggregate.Create(Guid.NewGuid(), 
         //   command.Batch.ProductId, 
         //   command.Batch.ProductSkuId, 
         //   command.Batch.WarehouseId, 
         //   userId);
        
        //await dbContext.Inventories.AddAsync(inventory);

        //var batchStock = BatchStock.Created(Guid.NewGuid(), batch.Id, userId);
        //await dbContext.BatchStocks.AddAsync(batchStock);

        //inventory.AddBatch(batchStock);
        //await dbContext.Inventories.AddAsync(inventory);


        //inventory.TransferIn(batchStock.Id, batchStock.Quantity, userId);

        ////movement
        //var movement = StockMovement.Create(
        //    Guid.NewGuid(),
        //    inventory.WarehouseId,
        //    batchStock.BatchId,
        //    inventory.ProductId,
        //    inventory.ProductSkuId,
        //    batchStock.Quantity,
        //    batchStock.Id,
        //    DateTime.UtcNow,
        //    MovementType.OPENING,
        //    MovementDirection.IN,
        //    MovementCategory.Physical,
        //    userId,
        //    "Add BatchStock");
        ////snapshot
        //var snapshot = InventorySnapshot.Create(Guid.NewGuid(),
        //    inventory.ProductId,
        //    inventory.ProductSkuId,
        //    inventory.WarehouseId,
        //    batchStock.BatchId,
        //    batchStock.Quantity,
        //    0,
        //    userId);


        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateBatchResult(true);
    }
}
