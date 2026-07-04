using Shared.Contracts.CQRS;

namespace Inventory.Contracts.Stock;

public record GetSkuLocationAvailabilityQuery(
    Guid CompanyId,
    Guid ProductSkuId,
    Guid? WarehouseId = null,
    Guid? WarehouseLocationId = null,
    Guid? BatchId = null,
    Guid? BranchId = null) : IQuery<GetSkuLocationAvailabilityResult>;

public record GetSkuLocationAvailabilityResult(
    Guid CompanyId,
    Guid ProductSkuId,
    Guid? WarehouseId,
    Guid? WarehouseLocationId,
    Guid? BatchId,
    decimal TotalQuantity,
    decimal ReservedQuantity,
    decimal AvailableQuantity,
    IReadOnlyList<SkuLocationAvailabilityRow> Rows);

public record SkuLocationAvailabilityRow(
    Guid Id,
    Guid CompanyId,
    Guid ProductId,
    Guid ProductSkuId,
    Guid WarehouseId,
    string? WarehouseName,
    string? WarehouseNameEng,
    Guid WarehouseLocationId,
    string? WarehouseLocationCode,
    string? WarehouseLocationName,
    string? WarehouseLocationNameEng,
    Guid BatchId,
    string? BatchNumber,
    DateTime? ExpiryDate,
    decimal Quantity,
    decimal ReservedQuantity,
    decimal AvailableQuantity);
