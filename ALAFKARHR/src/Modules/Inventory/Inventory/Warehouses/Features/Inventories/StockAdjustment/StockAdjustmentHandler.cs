using Catalog.Contracts.Products.Features.GetProductById;
using Accounting.Contracts.Accounting.Features;
using Inventory.Warehouses.Features.Inventories.StockIn;
using Shared.Exceptions;
using SharedWithUI.Accounting.Dtos;
using SharedWithUI.Accounting.Enums;

namespace Inventory.Warehouses.Features.Inventories.StockAdjustment;


public record StockAdjustmentCommand(CreateInventoryAggregateDto InventoryAggregate) : ICommand<StockAdjustmentResult>;
public record StockAdjustmentResult(Guid Id);

public class StockInCommandValidator : AbstractValidator<StockAdjustmentCommand>
{
    public StockInCommandValidator()
    {
        RuleFor(x=> x.InventoryAggregate.ProductId).NotEmpty().WithMessage("Product is required");
        RuleFor(x=> x.InventoryAggregate.ProductSkuId).NotEmpty().WithMessage("Sku is required");
        RuleFor(x=> x.InventoryAggregate.WarehouseId).NotEmpty().WithMessage("Warehouse is required");
        RuleFor(x=> x.InventoryAggregate.InitialQuantity).GreaterThan(0).WithMessage("Quantity must be greater than zero");
        RuleFor(x=> x.InventoryAggregate.InitialBatchId).NotEmpty().WithMessage("Batch is required");
        RuleFor(x => x.InventoryAggregate.ReferenceNumber).NotEmpty().MaximumLength(120).WithMessage("Reference number is required");
        RuleFor(x => x.InventoryAggregate.SourceDocumentType)
            .NotEmpty()
            .MaximumLength(80)
            .Must(global::Inventory.Warehouses.Features.Inventories.InventorySourceDocumentTypes.IsAllowed)
            .WithMessage("Source document type is not allowed");
    }
}
public class StockAdjustmentHandler(InventoryDbContext dbContext, ISender sender, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<StockAdjustmentCommand, StockAdjustmentResult>
{
    public async Task<StockAdjustmentResult> Handle(StockAdjustmentCommand command, CancellationToken cancellationToken)
    {
        var warehouse = await global::Inventory.Warehouses.Features.Inventories.InventoryBranchScope.EnsureCanMutateWarehouseAsync(
            dbContext,
            sender,
            command.InventoryAggregate.CompanyId,
            command.InventoryAggregate.WarehouseId,
            cancellationToken);

        //var batch = await dbContext.Batches.AsNoTracking().FirstOrDefaultAsync(b => b.Id == command.InventoryAggregate.InitialBatchId, cancellationToken);
        //if (batch is null)
        //    throw new NotFoundException($"Batch not found: {command.InventoryAggregate.InitialBatchId}");

        //var warehouse = await dbContext.Warehouses.AsNoTracking().FirstOrDefaultAsync(w => w.Id == command.InventoryAggregate.WarehouseId, cancellationToken);
        //if (warehouse is null)
        //    throw new NotFoundException($"Warehouse not found: {command.InventoryAggregate.WarehouseId}");

        //var prodRes = await sender.Send(new GetProductByIdQuery(command.InventoryAggregate.ProductId.Value));
        //if (prodRes.Product is null)
        //    throw new NotFoundException($"Product not found: {command.InventoryAggregate.ProductId}");

        //var sku = prodRes.Product.Skus.FirstOrDefault(s => s.Id == command.InventoryAggregate.ProductSkuId);
        //if (sku is null)
        //    throw new NotFoundException($"SKU not found: {command.InventoryAggregate.ProductSkuId}");

        var packageQuantity = await global::Inventory.Warehouses.Features.Inventories.InventoryPackageQuantityResolver
            .ResolveAsync(sender, command.InventoryAggregate, cancellationToken);

        var inventory = await dbContext.Inventories.Include(i => i.Batches)
                                .FirstOrDefaultAsync(i => i.WarehouseId == command.InventoryAggregate.WarehouseId &&
                                                         i.ProductSkuId == command.InventoryAggregate.ProductSkuId, cancellationToken);

        var userId = httpContextAccessor.HttpContext
                        .User?
                        .FindFirst(ClaimTypes.NameIdentifier)?
                        .Value ?? throw new UnauthorizedAccessException("User is not authenticated");

        decimal quantityBefore = 0;
        decimal reservedBefore = 0;
        if (inventory is null)
        {
            throw new NotFoundException("Inventory not found");
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

        }
        else
        {
            quantityBefore = inventory.TotalQuantity;
            reservedBefore = inventory.TotalReserved;
            if (command.InventoryAggregate.MovementType == MovementType.AdjustmentIncrease)
            {
                var destinationLocationId = await InventoryLocationBalanceService.ResolveDestinationLocationAsync(
                    dbContext,
                    command.InventoryAggregate.CompanyId,
                    command.InventoryAggregate.WarehouseId.Value,
                    command.InventoryAggregate.ProductId.Value,
                    command.InventoryAggregate.ProductSkuId.Value,
                    command.InventoryAggregate.DestinationLocationId,
                    cancellationToken);
                inventory.StockIn(new BatchStock(
                command.InventoryAggregate.InitialBatchId,
                command.InventoryAggregate.WarehouseId.Value,
                packageQuantity.NormalizedQuantity,
                userId));
                await InventoryLocationBalanceService.IncreaseAsync(
                    dbContext,
                    command.InventoryAggregate.CompanyId,
                    command.InventoryAggregate.ProductId.Value,
                    command.InventoryAggregate.ProductSkuId.Value,
                    command.InventoryAggregate.WarehouseId.Value,
                    destinationLocationId,
                    command.InventoryAggregate.InitialBatchId,
                    packageQuantity.NormalizedQuantity,
                    userId,
                    cancellationToken);
                command.InventoryAggregate.DestinationLocationId = destinationLocationId;
            }
            else
            {
                var sourceLocationId = await InventoryLocationBalanceService.ResolveSourceLocationAsync(
                    dbContext,
                    command.InventoryAggregate.CompanyId,
                    command.InventoryAggregate.WarehouseId.Value,
                    command.InventoryAggregate.ProductSkuId.Value,
                    command.InventoryAggregate.InitialBatchId,
                    command.InventoryAggregate.SourceLocationId,
                    packageQuantity.NormalizedQuantity,
                    requireReserved: false,
                    cancellationToken);
                inventory.StockOut(new BatchStock(
                command.InventoryAggregate.InitialBatchId,
                command.InventoryAggregate.WarehouseId.Value,
                packageQuantity.NormalizedQuantity,
                userId));
                await InventoryLocationBalanceService.DecreaseAsync(
                    dbContext,
                    command.InventoryAggregate.CompanyId,
                    command.InventoryAggregate.ProductSkuId.Value,
                    command.InventoryAggregate.WarehouseId.Value,
                    sourceLocationId,
                    command.InventoryAggregate.InitialBatchId,
                    packageQuantity.NormalizedQuantity,
                    userId,
                    cancellationToken);
                command.InventoryAggregate.SourceLocationId = sourceLocationId;
            }
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
            command.InventoryAggregate.CurrencyId!.Value,
            command.InventoryAggregate.ReferenceNumber!,
            command.InventoryAggregate.SourceDocumentType!,
            command.InventoryAggregate.MovementType,
            command.InventoryAggregate.MovementType==MovementType.AdjustmentIncrease? MovementDirection.IN:MovementDirection.OUT,
            userId,
            command.InventoryAggregate.Notes ?? string.Empty,
            productPackageId: packageQuantity.ProductPackageId,
            unitId: packageQuantity.UnitId,
            enteredQuantity: packageQuantity.EnteredQuantity,
            packageMultiplier: packageQuantity.PackageMultiplier,
            unitMultiplier: packageQuantity.UnitMultiplier,
            normalizedQuantity: packageQuantity.NormalizedQuantity,
            sourceDocumentId: command.InventoryAggregate.SourceDocumentId,
            sourceDocumentLineId: command.InventoryAggregate.SourceDocumentLineId,
            parentProductSkuId: command.InventoryAggregate.ParentProductSkuId,
            parentSalesOrderLineId: command.InventoryAggregate.ParentSalesOrderLineId,
            sourceLocationId: command.InventoryAggregate.SourceLocationId,
            destinationLocationId: command.InventoryAggregate.DestinationLocationId);
        await dbContext.StockMovements.AddAsync(movement, cancellationToken);
        await dbContext.InventoryValuationLayers.AddAsync(
            InventoryValuationLayer.FromMovement(movement, command.InventoryAggregate.CompanyId, userId),
            cancellationToken);


        await dbContext.SaveChangesAsync(cancellationToken);

        if (command.InventoryAggregate.TotalCost > 0)
        {
            var lines = new List<JournalEntryLineDto>();
            if (command.InventoryAggregate.MovementType == MovementType.AdjustmentIncrease)
            {
                lines.Add(new JournalEntryLineDto { AccountRole = AccountRole.Inventory, Debit = command.InventoryAggregate.TotalCost, Description = "Inventory adjustment increase" });
                lines.Add(new JournalEntryLineDto { AccountRole = AccountRole.Suspense, Credit = command.InventoryAggregate.TotalCost, Description = "Inventory adjustment clearing" });
            }
            else
            {
                lines.Add(new JournalEntryLineDto { AccountRole = AccountRole.Expense, Debit = command.InventoryAggregate.TotalCost, Description = "Inventory adjustment decrease" });
                lines.Add(new JournalEntryLineDto { AccountRole = AccountRole.Inventory, Credit = command.InventoryAggregate.TotalCost, Description = "Inventory adjustment decrease" });
            }

            await sender.Send(new CreateAndPostJournalEntryCommand(new CreateJournalEntryDto
            {
                CompanyId = command.InventoryAggregate.CompanyId,
                BranchId = warehouse.BranchId,
                EntryDate = DateTime.UtcNow,
                SourceModule = "Inventory",
                SourceDocumentId = movement.Id,
                SourceDocumentNumber = command.InventoryAggregate.ReferenceNumber,
                Memo = command.InventoryAggregate.Notes ?? command.InventoryAggregate.MovementType.ToString(),
                Lines = lines
            }), cancellationToken);
        }

        return new StockAdjustmentResult(inventory.Id);
    }
}
