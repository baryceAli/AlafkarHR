using Shared.Contracts.CQRS;

namespace Inventory.Contracts.Stock;

public record InventorySerialSelection(
    Guid? InventorySerialNumberId,
    string SerialNumber,
    Guid? BatchId = null,
    Guid? WarehouseId = null,
    Guid? WarehouseLocationId = null);

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
    string? SourceDocumentType = null,
    Guid? UnitId = null,
    Guid? SourceDocumentId = null,
    Guid? SourceDocumentLineId = null,
    Guid? ParentProductSkuId = null,
    Guid? ParentSalesOrderLineId = null,
    Guid? DestinationLocationId = null,
    IReadOnlyList<InventorySerialSelection>? SerialNumbers = null) : ICommand<PostInventoryStockResult>;

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
    bool ConsumeReservedQuantity = false,
    Guid? UnitId = null,
    Guid? SourceDocumentId = null,
    Guid? SourceDocumentLineId = null,
    Guid? ParentProductSkuId = null,
    Guid? ParentSalesOrderLineId = null,
    Guid? SourceLocationId = null,
    IReadOnlyList<InventorySerialSelection>? SerialNumbers = null) : ICommand<PostInventoryStockResult>;

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
    string? SourceDocumentType = null,
    Guid? UnitId = null,
    Guid? SourceDocumentId = null,
    Guid? SourceDocumentLineId = null,
    Guid? ParentProductSkuId = null,
    Guid? ParentSalesOrderLineId = null,
    Guid? SourceLocationId = null,
    IReadOnlyList<InventorySerialSelection>? SerialNumbers = null) : ICommand<PostInventoryStockResult>;

public record PostInventoryStockResult(Guid InventoryId);

public record PostInventoryReservationCommand(
    Guid ProductId,
    Guid ProductSkuId,
    Guid WarehouseId,
    Guid BatchId,
    decimal Quantity,
    Guid CompanyId,
    string? Notes,
    string? ReferenceNumber = null,
    string? SourceDocumentType = null,
    Guid? UnitId = null,
    Guid? CurrencyId = null,
    Guid? SourceDocumentId = null,
    Guid? SourceDocumentLineId = null,
    Guid? ParentProductSkuId = null,
    Guid? ParentSalesOrderLineId = null,
    Guid? SourceLocationId = null,
    IReadOnlyList<InventorySerialSelection>? SerialNumbers = null) : ICommand<PostInventoryStockResult>;

public record PostInventoryReleaseCommand(
    Guid ProductId,
    Guid ProductSkuId,
    Guid WarehouseId,
    Guid BatchId,
    decimal Quantity,
    Guid CompanyId,
    string? Notes,
    string? ReferenceNumber = null,
    string? SourceDocumentType = null,
    Guid? UnitId = null,
    Guid? CurrencyId = null,
    Guid? SourceDocumentId = null,
    Guid? SourceDocumentLineId = null,
    Guid? ParentProductSkuId = null,
    Guid? ParentSalesOrderLineId = null,
    Guid? SourceLocationId = null,
    IReadOnlyList<InventorySerialSelection>? SerialNumbers = null) : ICommand<PostInventoryStockResult>;
