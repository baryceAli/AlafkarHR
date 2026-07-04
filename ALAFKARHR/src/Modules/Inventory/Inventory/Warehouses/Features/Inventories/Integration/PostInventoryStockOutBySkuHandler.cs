using Inventory.Contracts.Stock;
using Inventory.Warehouses.Features.Inventories.StockOut;
using Shared.Contracts.CQRS;
using SharedWithUI.Inventory.Dtos;
using SharedWithUI.Inventory.Enums;

namespace Inventory.Warehouses.Features.Inventories.Integration;

public class PostInventoryStockOutBySkuHandler(InventoryDbContext dbContext, ISender sender)
    : ICommandHandler<PostInventoryStockOutBySkuCommand, PostInventoryStockResult>
{
    public async Task<PostInventoryStockResult> Handle(PostInventoryStockOutBySkuCommand command, CancellationToken cancellationToken)
    {
        await global::Inventory.Warehouses.Features.Inventories.InventoryBranchScope.EnsureCanMutateWarehouseAsync(
            dbContext,
            sender,
            command.CompanyId,
            command.WarehouseId,
            cancellationToken);

        var inventory = await dbContext.Inventories
            .Include(x => x.Batches)
            .ThenInclude(x => x.Batch)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.CompanyId == command.CompanyId
                && x.WarehouseId == command.WarehouseId
                && x.ProductSkuId == command.ProductSkuId, cancellationToken)
            ?? throw new BadRequestException("No inventory could be found for the StoreFront default warehouse.");

        if (inventory.TotalAvailable < command.Quantity)
            throw new BadRequestException("Insufficient stock in the StoreFront default warehouse.");

        var remaining = command.Quantity;
        Guid lastInventoryId = inventory.Id;
        var today = DateTime.UtcNow.Date;
        var allocations = inventory.Batches
            .Where(x => x.Available > 0 && x.Batch.ExpiryDate.Date >= today)
            .OrderBy(x => x.Batch.ExpiryDate)
            .ThenBy(x => x.CreatedAt ?? DateTime.MaxValue)
            .ToList();

        foreach (var batch in allocations)
        {
            if (remaining <= 0)
                break;

            var take = Math.Min(batch.Available, remaining);
            var result = await sender.Send(new StockOutCommand(new CreateInventoryAggregateDto
            {
                ProductId = command.ProductId,
                ProductSkuId = command.ProductSkuId,
                ProductPackageId = command.ProductPackageId,
                UnitId = command.UnitId,
                WarehouseId = command.WarehouseId,
                InitialBatchId = batch.BatchId,
                InitialQuantity = take,
                MovementType = MovementType.SalesShipment,
                UnitCost = command.UnitCost,
                TotalCost = command.Quantity == 0 ? 0 : command.TotalCost * (take / command.Quantity),
                CurrencyId = command.CurrencyId ?? Guid.Empty,
                CompanyId = command.CompanyId,
                Notes = command.Notes,
                ReferenceNumber = command.ReferenceNumber ?? $"POS-{command.WarehouseId:N}-{command.ProductSkuId:N}",
                SourceDocumentType = command.SourceDocumentType ?? "POSDirectSale",
                SourceDocumentId = command.SourceDocumentId,
                SourceDocumentLineId = command.SourceDocumentLineId,
                ParentProductSkuId = command.ParentProductSkuId,
                ParentSalesOrderLineId = command.ParentSalesOrderLineId
            }), cancellationToken);

            lastInventoryId = result.Id;
            remaining -= take;
        }

        if (remaining > 0)
            throw new BadRequestException("Insufficient stock in the StoreFront default warehouse.");

        return new PostInventoryStockResult(lastInventoryId);
    }
}
