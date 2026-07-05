using Shared.Contracts.CQRS;

namespace Inventory.Contracts.Stock;

public record GetSkuSerialAvailabilityQuery(
    Guid CompanyId,
    Guid ProductSkuId,
    Guid? WarehouseId = null,
    Guid? WarehouseLocationId = null,
    Guid? BatchId = null,
    Guid? BranchId = null) : IQuery<SkuSerialAvailabilityResult>;

public record GetSerialNumberTraceQuery(
    Guid CompanyId,
    Guid? ProductSkuId = null,
    string? SerialNumber = null) : IQuery<SerialNumberTraceResult>;

public record SkuSerialAvailabilityResult(
    Guid CompanyId,
    Guid ProductSkuId,
    Guid? WarehouseId,
    Guid? WarehouseLocationId,
    Guid? BatchId,
    int TotalCount,
    int ReservedCount,
    int AvailableCount,
    IReadOnlyList<InventorySerialAvailabilityRow> Serials);

public record InventorySerialAvailabilityRow(
    Guid Id,
    Guid CompanyId,
    Guid ProductId,
    Guid ProductSkuId,
    string SerialNumber,
    Guid? BatchId,
    string? BatchNumber,
    DateTime? ExpiryDate,
    Guid? WarehouseId,
    string? WarehouseName,
    string? WarehouseNameEng,
    Guid? WarehouseLocationId,
    string? WarehouseLocationCode,
    string? WarehouseLocationName,
    string? WarehouseLocationNameEng,
    int Status,
    Guid? SourceDocumentId,
    Guid? SourceDocumentLineId,
    Guid? LastStockMovementId,
    DateTime? LastMovementAt);

public record SerialNumberTraceResult(
    Guid InventorySerialNumberId,
    string SerialNumber,
    Guid ProductSkuId,
    int CurrentStatus,
    IReadOnlyList<SerialTraceMovementRow> Movements);

public record SerialTraceMovementRow(
    Guid Id,
    Guid ProductId,
    Guid ProductSkuId,
    Guid WarehouseId,
    Guid BatchId,
    string ReferenceNumber,
    string SourceDocumentType,
    decimal NormalizedQuantity,
    DateTime? CreatedAt);
