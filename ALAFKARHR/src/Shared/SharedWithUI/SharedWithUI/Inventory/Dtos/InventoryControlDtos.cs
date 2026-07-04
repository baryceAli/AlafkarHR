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
    public bool IsActive { get; set; } = true;
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

public enum WarehouseLocationType
{
    Storage = 1,
    Receiving = 2,
    Quality = 3,
    Picking = 4,
    Scrap = 5
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
