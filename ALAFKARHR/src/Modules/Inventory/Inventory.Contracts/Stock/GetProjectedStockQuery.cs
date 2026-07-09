using Shared.Contracts.CQRS;

namespace Inventory.Contracts.Stock;

public record GetProjectedStockQuery(
    Guid CompanyId,
    Guid? BranchId = null,
    Guid? WarehouseId = null,
    Guid? ProductSkuId = null) : IQuery<GetProjectedStockResult>;

public record GetProjectedStockResult(IReadOnlyCollection<ProjectedStockRow> Rows);

public record ProjectedStockRow(
    Guid ProductSkuId,
    Guid WarehouseId,
    Guid? BranchId,
    decimal OnHandQuantity,
    decimal ReservedQuantity,
    decimal AvailableQuantity,
    decimal IncomingQuantity,
    decimal OutgoingQuantity,
    decimal ForecastedQuantity);
