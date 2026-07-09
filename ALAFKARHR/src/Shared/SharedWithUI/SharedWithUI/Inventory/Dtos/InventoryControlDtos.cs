using SharedWithUI.Inventory.Enums;

namespace SharedWithUI.Inventory.Dtos;

public class WarehouseLocationDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid WarehouseId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string NameEng { get; set; } = string.Empty;
    public string? ParentCode { get; set; }
    public WarehouseLocationType LocationType { get; set; } = WarehouseLocationType.Storage;
    public WarehouseLocationUsage LocationUsage { get; set; } = WarehouseLocationUsage.Internal;
    public bool IsVirtual { get; set; }
    public bool ExcludeFromPhysicalStock { get; set; }
    public bool IsActive { get; set; } = true;
}

public class InventoryOperationTypeDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid WarehouseId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string NameEng { get; set; } = string.Empty;
    public InventoryOperationKind OperationKind { get; set; } = InventoryOperationKind.InternalTransfer;
    public Guid? DefaultSourceLocationId { get; set; }
    public Guid? DefaultDestinationLocationId { get; set; }
    public bool IsActive { get; set; } = true;
}

public class InventoryRouteDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid? WarehouseId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string NameEng { get; set; } = string.Empty;
    public InventoryRouteApplicationScope ApplicationScope { get; set; } = InventoryRouteApplicationScope.Warehouse;
    public Guid? ProductId { get; set; }
    public Guid? ProductSkuId { get; set; }
    public Guid? ProductCategoryId { get; set; }
    public bool IsActive { get; set; } = true;
}

public class InventoryRouteRuleDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid RouteId { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid OperationTypeId { get; set; }
    public InventoryRouteRuleAction Action { get; set; } = InventoryRouteRuleAction.Push;
    public Guid SourceLocationId { get; set; }
    public Guid DestinationLocationId { get; set; }
    public Guid? ProductId { get; set; }
    public Guid? ProductSkuId { get; set; }
    public Guid? ProductCategoryId { get; set; }
    public int Priority { get; set; }
    public bool IsActive { get; set; } = true;
}

public class InventoryRouteProposalDto
{
    public Guid RouteRuleId { get; set; }
    public Guid RouteId { get; set; }
    public string? RouteName { get; set; }
    public string? RouteNameEng { get; set; }
    public Guid OperationTypeId { get; set; }
    public string? OperationTypeCode { get; set; }
    public string? OperationTypeName { get; set; }
    public string? OperationTypeNameEng { get; set; }
    public InventoryRouteRuleAction Action { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid SourceLocationId { get; set; }
    public string? SourceLocationCode { get; set; }
    public string? SourceLocationName { get; set; }
    public string? SourceLocationNameEng { get; set; }
    public Guid DestinationLocationId { get; set; }
    public string? DestinationLocationCode { get; set; }
    public string? DestinationLocationName { get; set; }
    public string? DestinationLocationNameEng { get; set; }
    public int Priority { get; set; }
}

public class PutawayRuleDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid? ProductId { get; set; }
    public Guid? ProductSkuId { get; set; }
    public Guid? DestinationLocationId { get; set; }
    public RemovalStrategy RemovalStrategy { get; set; } = RemovalStrategy.Fifo;
    public int Priority { get; set; }
    public bool IsActive { get; set; } = true;
}

public class QualityInspectionDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid? SourceDocumentId { get; set; }
    public string? SourceDocumentNumber { get; set; }
    public Guid ProductId { get; set; }
    public Guid ProductSkuId { get; set; }
    public Guid? BatchId { get; set; }
    public Guid? WarehouseId { get; set; }
    public decimal Quantity { get; set; }
    public QualityInspectionStatus Status { get; set; } = QualityInspectionStatus.Pending;
    public string? ResultNotes { get; set; }
    public DateTime InspectionDate { get; set; } = DateTime.UtcNow;
}

public class LandedCostVoucherDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid SourceDocumentId { get; set; }
    public string SourceDocumentNumber { get; set; } = string.Empty;
    public Guid? CurrencyId { get; set; }
    public LandedCostAllocationMethod AllocationMethod { get; set; } = LandedCostAllocationMethod.ByValue;
    public decimal FreightAmount { get; set; }
    public decimal CustomsAmount { get; set; }
    public decimal HandlingAmount { get; set; }
    public decimal OtherAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public bool IsPosted { get; set; }
    public DateTime VoucherDate { get; set; } = DateTime.UtcNow;
}

public class InventoryValuationLayerDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid ProductId { get; set; }
    public Guid ProductSkuId { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid? BatchId { get; set; }
    public string SourceDocumentType { get; set; } = string.Empty;
    public string ReferenceNumber { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalCost { get; set; }
    public DateTime LayerDate { get; set; } = DateTime.UtcNow;
}

public class InventoryLocationBalanceDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? ProductNameEng { get; set; }
    public Guid ProductSkuId { get; set; }
    public string? ProductSkuName { get; set; }
    public string? ProductSkuNameEng { get; set; }
    public Guid WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
    public string? WarehouseNameEng { get; set; }
    public Guid WarehouseLocationId { get; set; }
    public string? WarehouseLocationCode { get; set; }
    public string? WarehouseLocationName { get; set; }
    public string? WarehouseLocationNameEng { get; set; }
    public Guid BatchId { get; set; }
    public string? BatchNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public decimal Quantity { get; set; }
    public decimal ReservedQuantity { get; set; }
    public decimal AvailableQuantity { get; set; }
}

public class LocationAvailabilityDto
{
    public Guid CompanyId { get; set; }
    public Guid ProductSkuId { get; set; }
    public Guid? WarehouseId { get; set; }
    public Guid? WarehouseLocationId { get; set; }
    public Guid? BatchId { get; set; }
    public decimal TotalQuantity { get; set; }
    public decimal ReservedQuantity { get; set; }
    public decimal AvailableQuantity { get; set; }
    public List<InventoryLocationBalanceDto> Rows { get; set; } = [];
}

public class InventorySerialNumberDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid ProductId { get; set; }
    public Guid ProductSkuId { get; set; }
    public string SerialNumber { get; set; } = string.Empty;
    public Guid? BatchId { get; set; }
    public string? BatchNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public Guid? WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
    public string? WarehouseNameEng { get; set; }
    public Guid? WarehouseLocationId { get; set; }
    public string? WarehouseLocationCode { get; set; }
    public string? WarehouseLocationName { get; set; }
    public string? WarehouseLocationNameEng { get; set; }
    public InventorySerialStatus Status { get; set; } = InventorySerialStatus.Available;
    public bool IsReserved => Status == InventorySerialStatus.Reserved;
    public bool IsAvailable => Status == InventorySerialStatus.Available || Status == InventorySerialStatus.Returned;
    public Guid? SourceDocumentId { get; set; }
    public Guid? SourceDocumentLineId { get; set; }
    public Guid? LastStockMovementId { get; set; }
    public DateTime? LastMovementAt { get; set; }
}

public class SkuSerialAvailabilityDto
{
    public Guid CompanyId { get; set; }
    public Guid ProductSkuId { get; set; }
    public Guid? WarehouseId { get; set; }
    public Guid? WarehouseLocationId { get; set; }
    public Guid? BatchId { get; set; }
    public int TotalCount { get; set; }
    public int ReservedCount { get; set; }
    public int AvailableCount { get; set; }
    public List<InventorySerialNumberDto> Serials { get; set; } = [];
}

public class SerialNumberTraceDto
{
    public Guid InventorySerialNumberId { get; set; }
    public string SerialNumber { get; set; } = string.Empty;
    public Guid ProductSkuId { get; set; }
    public InventorySerialStatus CurrentStatus { get; set; }
    public List<StockMovementDto> Movements { get; set; } = [];
}

public class CycleCountDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
    public string? WarehouseNameEng { get; set; }
    public Guid WarehouseLocationId { get; set; }
    public string? WarehouseLocationName { get; set; }
    public string? WarehouseLocationNameEng { get; set; }
    public string CountNumber { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public bool IsPosted { get; set; }
    public DateTime CountDate { get; set; } = DateTime.UtcNow;
    public List<CycleCountLineDto> Lines { get; set; } = [];
}

public class CycleCountLineDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? ProductNameEng { get; set; }
    public Guid ProductSkuId { get; set; }
    public string? ProductSkuName { get; set; }
    public string? ProductSkuNameEng { get; set; }
    public Guid BatchId { get; set; }
    public string? BatchNumber { get; set; }
    public decimal CountedQuantity { get; set; }
    public List<string> SerialNumbers { get; set; } = [];
    public string? Notes { get; set; }
}

public class PostCycleCountDto
{
    public Guid CycleCountId { get; set; }
}

public class ProjectedStockRowDto
{
    public Guid ProductSkuId { get; set; }
    public Guid WarehouseId { get; set; }
    public decimal OnHandQuantity { get; set; }
    public decimal ReservedQuantity { get; set; }
    public decimal AvailableQuantity { get; set; }
    public decimal IncomingQuantity { get; set; }
    public decimal OutgoingQuantity { get; set; }
    public decimal ForecastedQuantity { get; set; }
}

public class PutawaySuggestionDto
{
    public Guid CompanyId { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid ProductId { get; set; }
    public Guid ProductSkuId { get; set; }
    public Guid? PutawayRuleId { get; set; }
    public Guid? DestinationLocationId { get; set; }
    public string? DestinationLocationCode { get; set; }
    public string? DestinationLocationName { get; set; }
    public string? DestinationLocationNameEng { get; set; }
    public int? Priority { get; set; }
    public string? Warning { get; set; }
}

public class TransferFefoBatchSuggestionDto
{
    public Guid BatchId { get; set; }
    public string BatchNumber { get; set; } = string.Empty;
    public DateTime? ExpiryDate { get; set; }
    public decimal AvailableQuantity { get; set; }
    public decimal SuggestedQuantity { get; set; }
    public Guid? WarehouseLocationId { get; set; }
    public string? LocationCode { get; set; }
    public string? LocationName { get; set; }
    public string? LocationNameEng { get; set; }
}

public class InventoryOperationDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid? BranchId { get; set; }
    public Guid WarehouseId { get; set; }
    public InventoryOperationFlowDirection FlowDirection { get; set; } = InventoryOperationFlowDirection.Receipt;
    public InventoryOperationKind OperationKind { get; set; } = InventoryOperationKind.Receipt;
    public InventoryOperationStatus Status { get; set; } = InventoryOperationStatus.Ready;
    public int Sequence { get; set; }
    public bool IsStockPostingStep { get; set; }
    public bool StockPosted { get; set; }
    public string SourceDocumentType { get; set; } = string.Empty;
    public Guid SourceDocumentId { get; set; }
    public string SourceDocumentNumber { get; set; } = string.Empty;
    public Guid? BackorderOfOperationId { get; set; }
    public decimal PlannedQuantity { get; set; }
    public decimal DoneQuantity { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? CompletedBy { get; set; }
    public List<InventoryOperationLineDto> Lines { get; set; } = [];
}

public class InventoryOperationLineDto
{
    public Guid Id { get; set; }
    public Guid InventoryOperationId { get; set; }
    public int LineNumber { get; set; }
    public Guid ProductId { get; set; }
    public Guid ProductSkuId { get; set; }
    public Guid? ProductPackageId { get; set; }
    public Guid? UnitId { get; set; }
    public Guid BatchId { get; set; }
    public Guid? SourceLocationId { get; set; }
    public Guid? DestinationLocationId { get; set; }
    public Guid? SourceDocumentLineId { get; set; }
    public Guid? StockMovementId { get; set; }
    public decimal PlannedQuantity { get; set; }
    public decimal DoneQuantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalCost { get; set; }
    public Guid CurrencyId { get; set; }
    public string? Notes { get; set; }
    public bool ConsumeReservedQuantity { get; set; }
}

public class InventoryOperationFilterDto
{
    public Guid CompanyId { get; set; }
    public Guid? BranchId { get; set; }
    public Guid? WarehouseId { get; set; }
    public InventoryOperationStatus? Status { get; set; }
    public InventoryOperationKind? OperationKind { get; set; }
    public InventoryOperationFlowDirection? FlowDirection { get; set; }
    public string? SourceDocumentType { get; set; }
    public Guid? SourceDocumentId { get; set; }
}

public class ValidateInventoryOperationDto
{
    public List<ValidateInventoryOperationLineDto> Lines { get; set; } = [];
}

public class ValidateInventoryOperationLineDto
{
    public Guid LineId { get; set; }
    public decimal DoneQuantity { get; set; }
}

public class InventoryExecutionDashboardDto
{
    public List<InventoryExecutionMetricDto> Metrics { get; set; } = [];
    public List<InventoryOperationDto> OperationsToProcess { get; set; } = [];
    public List<InventoryOperationDto> Backorders { get; set; } = [];
    public List<PickingGroupDto> OpenPickingGroups { get; set; } = [];
    public List<BarcodeOperationSessionDto> RecentBarcodeSessions { get; set; } = [];
}

public class InventoryExecutionMetricDto
{
    public string Key { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string LabelEng { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string? Url { get; set; }
    public string? Icon { get; set; }
    public string? Tone { get; set; }
}

public class PickingGroupFilterDto
{
    public Guid CompanyId { get; set; }
    public Guid? BranchId { get; set; }
    public Guid? WarehouseId { get; set; }
    public PickingGroupStatus? Status { get; set; }
    public PickingGroupType? GroupType { get; set; }
}

public class CreatePickingGroupDto
{
    public Guid CompanyId { get; set; }
    public Guid? BranchId { get; set; }
    public Guid WarehouseId { get; set; }
    public PickingGroupType GroupType { get; set; } = PickingGroupType.Batch;
    public string? Name { get; set; }
    public string? ResponsibleUserId { get; set; }
    public string? DockLocation { get; set; }
    public string? Notes { get; set; }
    public List<Guid> InventoryOperationIds { get; set; } = [];
}

public class PickingGroupDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid? BranchId { get; set; }
    public Guid WarehouseId { get; set; }
    public PickingGroupType GroupType { get; set; }
    public PickingGroupStatus Status { get; set; }
    public string GroupNumber { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? ResponsibleUserId { get; set; }
    public string? DockLocation { get; set; }
    public string? Notes { get; set; }
    public decimal PlannedQuantity { get; set; }
    public decimal DoneQuantity { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public List<PickingGroupLineDto> Lines { get; set; } = [];
}

public class PickingGroupLineDto
{
    public Guid Id { get; set; }
    public Guid PickingGroupId { get; set; }
    public Guid InventoryOperationId { get; set; }
    public string SourceDocumentType { get; set; } = string.Empty;
    public Guid SourceDocumentId { get; set; }
    public string SourceDocumentNumber { get; set; } = string.Empty;
    public InventoryOperationKind OperationKind { get; set; }
    public InventoryOperationStatus OperationStatus { get; set; }
    public decimal PlannedQuantity { get; set; }
    public decimal DoneQuantity { get; set; }
}

public enum WarehouseLocationType
{
    Storage = 1,
    Receiving = 2,
    Quality = 3,
    Picking = 4,
    Scrap = 5,
    Packing = 6,
    Output = 7,
    Transit = 8
}

public enum WarehouseLocationUsage
{
    Internal = 1,
    Vendor = 2,
    Customer = 3,
    Transit = 4,
    Production = 5,
    VirtualScrap = 6
}

public enum InventoryOperationKind
{
    Receipt = 1,
    InternalTransfer = 2,
    Delivery = 3,
    Return = 4,
    Scrap = 5,
    Pick = 6,
    Pack = 7,
    QualityControl = 8
}

public enum InventoryRouteRuleAction
{
    Push = 1,
    Pull = 2
}

public enum InventoryRouteApplicationScope
{
    Warehouse = 1,
    Product = 2,
    ProductSku = 3,
    ProductCategory = 4
}

public enum InventoryOperationFlowDirection
{
    Receipt = 1,
    Delivery = 2
}

public enum InventoryOperationStatus
{
    Ready = 1,
    InProgress = 2,
    Done = 3,
    PartiallyDone = 4,
    Cancelled = 5
}

public enum PickingGroupType
{
    Batch = 1,
    Wave = 2
}

public enum PickingGroupStatus
{
    Draft = 1,
    Ready = 2,
    Processed = 3,
    Cancelled = 4
}

public enum RemovalStrategy
{
    Fifo = 1,
    Fefo = 2,
    Manual = 3
}

public enum QualityInspectionStatus
{
    Pending = 1,
    Passed = 2,
    Failed = 3,
    Waived = 4
}

public enum LandedCostAllocationMethod
{
    ByValue = 1,
    ByQuantity = 2,
    Equal = 3
}
