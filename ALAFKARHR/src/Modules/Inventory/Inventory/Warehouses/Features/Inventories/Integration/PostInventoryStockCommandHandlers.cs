using Accounting.Contracts.Accounting.Features;
using Inventory.Contracts.Stock;
using Inventory.Warehouses.Features.Inventories.StockRelease;
using Inventory.Warehouses.Features.Inventories.StockReservation;
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
        var movementType = command.SourceDocumentType == "SalesReturn"
            ? MovementType.CustomerReturn
            : MovementType.PurchaseReceipt;
        var result = await sender.Send(new StockInCommand(command.ToInventoryAggregateDto(movementType)), cancellationToken);
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
        var movementType = command.SourceDocumentType == "SalesDeliveryNote"
            ? MovementType.SalesShipment
            : MovementType.SupplierReturn;
        var result = await sender.Send(new StockOutCommand(command.ToInventoryAggregateDto(movementType)), cancellationToken);
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

public class PostInventoryReservationCommandHandler(ISender sender)
    : ICommandHandler<PostInventoryReservationCommand, PostInventoryStockResult>
{
    public async Task<PostInventoryStockResult> Handle(PostInventoryReservationCommand command, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new StockReservationCommand(command.ToInventoryAggregateDto(MovementType.ReserveAmount)), cancellationToken);
        return new PostInventoryStockResult(result.Id);
    }
}

public class PostInventoryReleaseCommandHandler(ISender sender)
    : ICommandHandler<PostInventoryReleaseCommand, PostInventoryStockResult>
{
    public async Task<PostInventoryStockResult> Handle(PostInventoryReleaseCommand command, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new StockReleaseCommand(command.ToInventoryAggregateDto(MovementType.ReleaseAmount)), cancellationToken);
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
            UnitId = command.UnitId,
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
            SourceDocumentId = command.SourceDocumentId,
            SourceDocumentLineId = command.SourceDocumentLineId,
            ParentProductSkuId = command.ParentProductSkuId,
            ParentSalesOrderLineId = command.ParentSalesOrderLineId,
            DestinationLocationId = command.DestinationLocationId,
            SerialNumbers = command.SerialNumbers.ToDtoList()
        };

    public static CreateInventoryAggregateDto ToInventoryAggregateDto(this PostInventoryStockOutCommand command, MovementType movementType) =>
        new()
        {
            ProductId = command.ProductId,
            ProductSkuId = command.ProductSkuId,
            ProductPackageId = command.ProductPackageId,
            UnitId = command.UnitId,
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
            ConsumeReservedQuantity = command.ConsumeReservedQuantity,
            SourceDocumentId = command.SourceDocumentId,
            SourceDocumentLineId = command.SourceDocumentLineId,
            ParentProductSkuId = command.ParentProductSkuId,
            ParentSalesOrderLineId = command.ParentSalesOrderLineId,
            SourceLocationId = command.SourceLocationId,
            SerialNumbers = command.SerialNumbers.ToDtoList()
        };

    public static CreateInventoryAggregateDto ToInventoryAggregateDto(this PostInventoryReservationCommand command, MovementType movementType) =>
        new()
        {
            ProductId = command.ProductId,
            ProductSkuId = command.ProductSkuId,
            UnitId = command.UnitId,
            WarehouseId = command.WarehouseId,
            InitialBatchId = command.BatchId,
            InitialQuantity = command.Quantity,
            MovementType = movementType,
            UnitCost = 0m,
            TotalCost = 0m,
            CurrencyId = command.CurrencyId ?? Guid.Empty,
            CompanyId = command.CompanyId,
            Notes = command.Notes,
            ReferenceNumber = command.ReferenceNumber ?? $"SalesOrderReservation-{command.WarehouseId:N}-{command.BatchId:N}",
            SourceDocumentType = command.SourceDocumentType ?? "SalesOrderReservation",
            SourceDocumentId = command.SourceDocumentId,
            SourceDocumentLineId = command.SourceDocumentLineId,
            ParentProductSkuId = command.ParentProductSkuId,
            ParentSalesOrderLineId = command.ParentSalesOrderLineId,
            SourceLocationId = command.SourceLocationId,
            SerialNumbers = command.SerialNumbers.ToDtoList()
        };

    public static CreateInventoryAggregateDto ToInventoryAggregateDto(this PostInventoryReleaseCommand command, MovementType movementType) =>
        new()
        {
            ProductId = command.ProductId,
            ProductSkuId = command.ProductSkuId,
            UnitId = command.UnitId,
            WarehouseId = command.WarehouseId,
            InitialBatchId = command.BatchId,
            InitialQuantity = command.Quantity,
            MovementType = movementType,
            UnitCost = 0m,
            TotalCost = 0m,
            CurrencyId = command.CurrencyId ?? Guid.Empty,
            CompanyId = command.CompanyId,
            Notes = command.Notes,
            ReferenceNumber = command.ReferenceNumber ?? $"SalesOrderReservationRelease-{command.WarehouseId:N}-{command.BatchId:N}",
            SourceDocumentType = command.SourceDocumentType ?? "SalesOrderReservationRelease",
            SourceDocumentId = command.SourceDocumentId,
            SourceDocumentLineId = command.SourceDocumentLineId,
            ParentProductSkuId = command.ParentProductSkuId,
            ParentSalesOrderLineId = command.ParentSalesOrderLineId,
            SourceLocationId = command.SourceLocationId,
            SerialNumbers = command.SerialNumbers.ToDtoList()
        };

    private static List<InventorySerialSelectionDto> ToDtoList(this IReadOnlyList<InventorySerialSelection>? serials) =>
        serials?
            .Select(x => new InventorySerialSelectionDto
            {
                InventorySerialNumberId = x.InventorySerialNumberId,
                SerialNumber = x.SerialNumber,
                BatchId = x.BatchId,
                WarehouseId = x.WarehouseId,
                WarehouseLocationId = x.WarehouseLocationId
            })
            .ToList() ?? [];
}
