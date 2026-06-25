using Shared.Contracts.CQRS;

namespace Inventory.Contracts.Stock;

public record PostInventoryStockInCommand(
    Guid ProductId,
    Guid ProductSkuId,
    Guid? ProductPackageId,
    Guid WarehouseId,
    Guid BatchId,
    decimal Quantity,
    decimal UnitCost,
    decimal TotalCost,
    Guid CurrencyId,
    Guid CompanyId,
    string? Notes,
    string? ReferenceNumber = null,
    string? SourceDocumentType = null) : ICommand<PostInventoryStockResult>;

public record PostInventoryStockOutCommand(
    Guid ProductId,
    Guid ProductSkuId,
    Guid? ProductPackageId,
    Guid WarehouseId,
    Guid BatchId,
    decimal Quantity,
    decimal UnitCost,
    decimal TotalCost,
    Guid CurrencyId,
    Guid CompanyId,
    string? Notes,
    string? ReferenceNumber = null,
    string? SourceDocumentType = null,
    bool ConsumeReservedQuantity = false) : ICommand<PostInventoryStockResult>;

public record PostInventoryStockOutBySkuCommand(
    Guid ProductId,
    Guid ProductSkuId,
    Guid? ProductPackageId,
    Guid WarehouseId,
    decimal Quantity,
    decimal UnitCost,
    decimal TotalCost,
    Guid? CurrencyId,
    Guid CompanyId,
    string? Notes,
    string? ReferenceNumber = null,
    string? SourceDocumentType = null) : ICommand<PostInventoryStockResult>;

public record PostInventoryStockResult(Guid InventoryId);
