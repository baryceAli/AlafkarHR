using Accounting.Contracts.Accounting.Features;
using Inventory.Contracts.Stock;
using Inventory.Warehouses.Features.Inventories.StockIn;
using Inventory.Warehouses.Features.Inventories.StockOut;
using Shared.Contracts.CQRS;
using SharedWithUI.Accounting.Dtos;
using SharedWithUI.Accounting.Enums;
using SharedWithUI.Inventory.Dtos;
using SharedWithUI.Inventory.Enums;

namespace Inventory.Warehouses.Features.Inventories.Integration;

public class PostInventoryStockInCommandHandler(ISender sender)
    : ICommandHandler<PostInventoryStockInCommand, PostInventoryStockResult>
{
    public async Task<PostInventoryStockResult> Handle(PostInventoryStockInCommand command, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new StockInCommand(command.ToInventoryAggregateDto(MovementType.PurchaseReceipt)), cancellationToken);
        if (command.TotalCost > 0)
        {
            await sender.Send(new CreateAndPostJournalEntryCommand(new CreateJournalEntryDto
            {
                CompanyId = command.CompanyId,
                EntryDate = DateTime.UtcNow,
                SourceModule = "Inventory",
                SourceDocumentNumber = command.ReferenceNumber ?? $"PurchaseReceipt-{result.Id:N}",
                Memo = command.Notes ?? "Inventory purchase receipt valuation",
                Lines =
                [
                    new() { AccountRole = AccountRole.Inventory, Debit = command.TotalCost, Description = "Inventory receipt" },
                    new() { AccountRole = AccountRole.Suspense, Credit = command.TotalCost, Description = "Goods received clearing" }
                ]
            }), cancellationToken);
        }
        return new PostInventoryStockResult(result.Id);
    }
}

public class PostInventoryStockOutCommandHandler(ISender sender)
    : ICommandHandler<PostInventoryStockOutCommand, PostInventoryStockResult>
{
    public async Task<PostInventoryStockResult> Handle(PostInventoryStockOutCommand command, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new StockOutCommand(command.ToInventoryAggregateDto(MovementType.SupplierReturn)), cancellationToken);
        if (command.TotalCost > 0)
        {
            await sender.Send(new CreateAndPostJournalEntryCommand(new CreateJournalEntryDto
            {
                CompanyId = command.CompanyId,
                EntryDate = DateTime.UtcNow,
                SourceModule = "Inventory",
                SourceDocumentNumber = command.ReferenceNumber ?? $"SupplierReturn-{result.Id:N}",
                Memo = command.Notes ?? "Inventory supplier return valuation",
                Lines =
                [
                    new() { AccountRole = AccountRole.Suspense, Debit = command.TotalCost, Description = "Goods returned clearing" },
                    new() { AccountRole = AccountRole.Inventory, Credit = command.TotalCost, Description = "Inventory return" }
                ]
            }), cancellationToken);
        }
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
            Notes = command.Notes,
            ReferenceNumber = command.ReferenceNumber ?? $"{movementType}-{command.WarehouseId:N}-{command.BatchId:N}",
            SourceDocumentType = command.SourceDocumentType ?? "Integration"
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
            Notes = command.Notes,
            ReferenceNumber = command.ReferenceNumber ?? $"{movementType}-{command.WarehouseId:N}-{command.BatchId:N}",
            SourceDocumentType = command.SourceDocumentType ?? "Integration",
            ConsumeReservedQuantity = command.ConsumeReservedQuantity
        };
}
