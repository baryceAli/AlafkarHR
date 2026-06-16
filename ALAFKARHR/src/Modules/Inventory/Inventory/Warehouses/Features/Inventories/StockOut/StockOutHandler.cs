using Catalog.Contracts.Products.Features.GetProductById;
using Shared.Exceptions;

namespace Inventory.Warehouses.Features.Inventories.StockOut;


public record StockOutCommand(CreateInventoryAggregateDto InventoryAggregate) : ICommand<StockOutResult>;
public record StockOutResult(Guid Id);

public class StockInCommandValidator : AbstractValidator<StockOutCommand>
{
    public StockInCommandValidator()
    {
        RuleFor(x=> x.InventoryAggregate.ProductId).NotEmpty().WithMessage("Product is required");
        RuleFor(x=> x.InventoryAggregate.ProductSkuId).NotEmpty().WithMessage("Sku is required");
        RuleFor(x=> x.InventoryAggregate.WarehouseId).NotEmpty().WithMessage("Warehouse is required");
        RuleFor(x=> x.InventoryAggregate.InitialQuantity).GreaterThanOrEqualTo(0).WithMessage("Quantity can not be negative");
        RuleFor(x=> x.InventoryAggregate.InitialBatchId).NotEmpty().WithMessage("Batch is required");
    }
}
public class StockOutHandler(InventoryDbContext dbContext, ISender sender, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<StockOutCommand, StockOutResult>
{
    public async Task<StockOutResult> Handle(StockOutCommand command, CancellationToken cancellationToken)
    {
        //var batch = await dbContext.Batches.AsNoTracking().FirstOrDefaultAsync(b => b.Id == command.InventoryAggregate.InitialBatchId, cancellationToken);
        //if (batch is null)
        //    throw new NotFoundException($"Batch not found: {command.InventoryAggregate.InitialBatchId}");

        //var warehouse=await dbContext.Warehouses.AsNoTracking().FirstOrDefaultAsync(w=>w.Id==command.InventoryAggregate.WarehouseId, cancellationToken);
        //if (warehouse is null)
        //    throw new NotFoundException($"Warehouse not found: {command.InventoryAggregate.WarehouseId}");

        var packageQuantity = await global::Inventory.Warehouses.Features.Inventories.InventoryPackageQuantityResolver
            .ResolveAsync(sender, command.InventoryAggregate, cancellationToken);

        var inventory = await dbContext.Inventories.Include(i=>i.Batches)
                                .FirstOrDefaultAsync(i => i.WarehouseId == command.InventoryAggregate.WarehouseId &&
                                                         i.ProductSkuId == command.InventoryAggregate.ProductSkuId, cancellationToken);

        var userId = httpContextAccessor.HttpContext
                        .User?
                        .FindFirst(ClaimTypes.NameIdentifier)?
                        .Value ?? throw new UnauthorizedAccessException("User is not authenticated");

        decimal quantityBefore = 0;
        decimal reservedBefore = 0;
        string reference = "";
        if(inventory is null)
        {
            throw new NotFoundException($"No inventory could be found");
            //create new inventory
            inventory = InventoryAggregate.Create(
                Guid.NewGuid(),
                command.InventoryAggregate.ProductId.Value,
                command.InventoryAggregate.ProductSkuId.Value,
                command.InventoryAggregate.WarehouseId.Value,
                command.InventoryAggregate.InitialBatchId,
                packageQuantity.NormalizedQuantity,
                command.InventoryAggregate.CompanyId,
                userId);

            var res = await dbContext.Inventories.AddAsync(inventory);
            reference = res.Entity.Id.ToString();
        }
        else
        {
            quantityBefore = inventory.TotalQuantity;
            reservedBefore = inventory.TotalReserved;
            
            inventory.StockOut(new BatchStock(
                command.InventoryAggregate.InitialBatchId,
                command.InventoryAggregate.WarehouseId.Value,
                packageQuantity.NormalizedQuantity,
                userId));
            reference=inventory.Id.ToString();
        }



        // Add movement
        // Later this should be done through DDD events
        var movement = StockMovement.Create(
            Guid.NewGuid(),
            command.InventoryAggregate.WarehouseId.Value,
            command.InventoryAggregate.InitialBatchId,
            command.InventoryAggregate.ProductId.Value,
            command.InventoryAggregate.ProductSkuId.Value,
            quantityBefore,
            inventory.TotalQuantity,
            reservedBefore,
            inventory.TotalReserved,
            command.InventoryAggregate.UnitCost,
            command.InventoryAggregate.TotalCost,
            command.InventoryAggregate.Currency.Value,
            reference,
            "InventoryAggregate",
            command.InventoryAggregate.MovementType,
            MovementDirection.OUT,
            userId,
            command.InventoryAggregate.Notes ?? string.Empty,
            productPackageId: packageQuantity.ProductPackageId,
            enteredQuantity: packageQuantity.EnteredQuantity,
            packageMultiplier: packageQuantity.PackageMultiplier,
            normalizedQuantity: packageQuantity.NormalizedQuantity);
 
        await dbContext.StockMovements.AddAsync(movement);


        await dbContext.SaveChangesAsync();

        return new StockOutResult(inventory.Id);
    }
}
