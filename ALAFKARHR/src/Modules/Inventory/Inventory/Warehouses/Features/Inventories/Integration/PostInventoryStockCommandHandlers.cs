using Inventory.Contracts.Stock;
using Inventory.Warehouses.Features.Inventories.StockIn;
using Inventory.Warehouses.Features.Inventories.StockOut;
using Shared.Contracts.CQRS;
using SharedWithUI.Inventory.Dtos;
using SharedWithUI.Inventory.Enums;

namespace Inventory.Warehouses.Features.Inventories.Integration;

public class PostInventoryStockInCommandHandler(ISender sender)
    : ICommandHandler<PostInventoryStockInCommand, PostInventoryStockResult>
{
    public async Task<PostInventoryStockResult> Handle(PostInventoryStockInCommand command, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new StockInCommand(command.ToInventoryAggregateDto(MovementType.PurchaseReceipt)), cancellationToken);
        return new PostInventoryStockResult(result.Id);
    }
}

public class PostInventoryStockOutCommandHandler(ISender sender)
    : ICommandHandler<PostInventoryStockOutCommand, PostInventoryStockResult>
{
    public async Task<PostInventoryStockResult> Handle(PostInventoryStockOutCommand command, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new StockOutCommand(command.ToInventoryAggregateDto(MovementType.SupplierReturn)), cancellationToken);
        return new PostInventoryStockResult(result.Id);
    }
}

internal static class InventoryStockCommandMapper
{
    public static CreateInventoryAggregateDto ToInventoryAggregateDto(this PostInventoryStockInCommand command, MovementType movementType) =>
        new()
        {
            ProductId = command.ProductId,
            ProductSkuId = command.ProductSkuId,
            ProductPackageId = command.ProductPackageId,
            WarehouseId = command.WarehouseId,
            InitialBatchId = command.BatchId,
            InitialQuantity = command.Quantity,
            MovementType = movementType,
            UnitCost = command.UnitCost,
            TotalCost = command.TotalCost,
            CurrencyId = command.CurrencyId,
            CompanyId = command.CompanyId,
            Notes = command.Notes
        };

    public static CreateInventoryAggregateDto ToInventoryAggregateDto(this PostInventoryStockOutCommand command, MovementType movementType) =>
        new()
        {
            ProductId = command.ProductId,
            ProductSkuId = command.ProductSkuId,
            ProductPackageId = command.ProductPackageId,
            WarehouseId = command.WarehouseId,
            InitialBatchId = command.BatchId,
            InitialQuantity = command.Quantity,
            MovementType = movementType,
            UnitCost = command.UnitCost,
            TotalCost = command.TotalCost,
            CurrencyId = command.CurrencyId,
            CompanyId = command.CompanyId,
            Notes = command.Notes
        };
}
