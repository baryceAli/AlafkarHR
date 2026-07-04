using Shared.Contracts.CQRS;

namespace Inventory.Contracts.Stock;

public record GetSkuAvailabilityQuery(
    Guid CompanyId,
    Guid ProductSkuId,
    Guid? WarehouseId = null,
    Guid? BranchId = null) : IQuery<GetSkuAvailabilityResult>;

public record GetSkuAvailabilityResult(
    Guid CompanyId,
    Guid ProductSkuId,
    Guid UnitId,
    string UnitName,
    string UnitNameEng,
    string UnitCategory,
    decimal UnitConversionFactor,
    decimal TotalQuantity,
    decimal ReservedQuantity,
    decimal AvailableQuantity,
    IReadOnlyList<SkuAvailabilityWarehouseRow> Warehouses);

public record SkuAvailabilityWarehouseRow(
    Guid WarehouseId,
    string WarehouseName,
    string WarehouseNameEng,
    Guid? BranchId,
    decimal TotalQuantity,
    decimal ReservedQuantity,
    decimal AvailableQuantity,
    IReadOnlyList<SkuAvailabilityBatchRow> Batches,
    IReadOnlyList<SkuAvailabilityLocationRow>? Locations = null);

public record SkuAvailabilityBatchRow(
    Guid BatchId,
    string BatchNumber,
    DateTime? ExpiryDate,
    decimal Quantity,
    decimal ReservedQuantity,
    decimal AvailableQuantity);

public record SkuAvailabilityLocationRow(
    Guid WarehouseLocationId,
    string? LocationCode,
    string? LocationName,
    string? LocationNameEng,
    Guid BatchId,
    string? BatchNumber,
    DateTime? ExpiryDate,
    decimal Quantity,
    decimal ReservedQuantity,
    decimal AvailableQuantity);
