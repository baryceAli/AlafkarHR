using Shared.Contracts.CQRS;

namespace Inventory.Contracts.Stock;

public record GetStockMovementsBySourceQuery(
    Guid CompanyId,
    string SourceDocumentType,
    Guid SourceDocumentId) : IQuery<GetStockMovementsBySourceResult>;

public record GetStockMovementsBySourceResult(IReadOnlyList<StockMovementTraceRow> Movements);

public record StockMovementTraceRow(
    Guid Id,
    Guid ProductId,
    Guid ProductSkuId,
    Guid WarehouseId,
    Guid BatchId,
    Guid? UnitId,
    string ReferenceNumber,
    string SourceDocumentType,
    Guid? SourceDocumentId,
    Guid? SourceDocumentLineId,
    Guid? ParentProductSkuId,
    Guid? ParentSalesOrderLineId,
    decimal QuantityBefore,
    decimal QuantityAfter,
    decimal NormalizedQuantity,
    decimal ReservedBefore,
    decimal ReservedAfter,
    DateTime? CreatedAt);
