namespace SharedWithUI.Inventory.Dtos;

public class BarcodeScanRequestDto
{
    public Guid CompanyId { get; set; }
    public string Barcode { get; set; } = string.Empty;
    public BarcodeOperationType OperationType { get; set; } = BarcodeOperationType.StockIn;
    public Guid? WarehouseId { get; set; }
    public Guid? WarehouseLocationId { get; set; }
    public Guid? ProductSkuId { get; set; }
    public bool IsOutbound { get; set; }
}

public class BarcodeScanResultDto
{
    public string RawBarcode { get; set; } = string.Empty;
    public string NormalizedBarcode { get; set; } = string.Empty;
    public BarcodeScanEntityType EntityType { get; set; } = BarcodeScanEntityType.Unknown;
    public Guid? EntityId { get; set; }
    public Guid? CompanyId { get; set; }
    public Guid? ProductId { get; set; }
    public Guid? ProductSkuId { get; set; }
    public Guid? ProductPackageId { get; set; }
    public Guid? WarehouseId { get; set; }
    public Guid? WarehouseLocationId { get; set; }
    public Guid? BatchId { get; set; }
    public Guid? InventorySerialNumberId { get; set; }
    public string? SerialNumber { get; set; }
    public string? Code { get; set; }
    public string? Label { get; set; }
    public string? LabelEng { get; set; }
    public string? BatchNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public decimal? Quantity { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsExpired { get; set; }
    public bool IsRejected { get; set; }
    public string? Warning { get; set; }
    public List<string> Warnings { get; set; } = [];
}

public class BarcodeOperationSessionDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public BarcodeOperationType OperationType { get; set; } = BarcodeOperationType.StockIn;
    public BarcodeSessionStatus Status { get; set; } = BarcodeSessionStatus.Draft;
    public Guid? WarehouseId { get; set; }
    public Guid? SourceLocationId { get; set; }
    public Guid? DestinationLocationId { get; set; }
    public string? SourceDocumentType { get; set; }
    public Guid? SourceDocumentId { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? Notes { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? AppliedAt { get; set; }
    public List<string> Warnings { get; set; } = [];
    public List<BarcodeOperationLineDto> Lines { get; set; } = [];
}

public class BarcodeOperationLineDto
{
    public Guid Id { get; set; }
    public Guid BarcodeOperationSessionId { get; set; }
    public string RawBarcode { get; set; } = string.Empty;
    public BarcodeScanEntityType EntityType { get; set; } = BarcodeScanEntityType.Unknown;
    public BarcodeLineStatus Status { get; set; } = BarcodeLineStatus.Accepted;
    public Guid? ProductId { get; set; }
    public Guid? ProductSkuId { get; set; }
    public Guid? ProductPackageId { get; set; }
    public Guid? BatchId { get; set; }
    public Guid? InventorySerialNumberId { get; set; }
    public string? SerialNumber { get; set; }
    public Guid? WarehouseId { get; set; }
    public Guid? SourceLocationId { get; set; }
    public Guid? DestinationLocationId { get; set; }
    public decimal EnteredQuantity { get; set; } = 1;
    public decimal PackageMultiplier { get; set; } = 1;
    public decimal UnitMultiplier { get; set; } = 1;
    public decimal NormalizedQuantity { get; set; } = 1;
    public string? DisplayLabel { get; set; }
    public string? DisplayLabelEng { get; set; }
    public string? BatchNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? Warning { get; set; }
    public DateTime ScannedAt { get; set; } = DateTime.UtcNow;
}

public class ApplyBarcodeSessionDto
{
    public Guid SessionId { get; set; }
    public bool ConfirmWarnings { get; set; }
}

public class BarcodeApplyResultDto
{
    public Guid SessionId { get; set; }
    public BarcodeSessionStatus Status { get; set; }
    public Guid? CreatedDocumentId { get; set; }
    public string? CreatedDocumentType { get; set; }
    public string? Message { get; set; }
}

public enum BarcodeScanEntityType
{
    Unknown = 0,
    ProductSku = 1,
    ProductPackage = 2,
    WarehouseLocation = 3,
    Batch = 4,
    WarehouseTransfer = 5,
    DeliveryNote = 6,
    GoodsReceipt = 7,
    CycleCount = 8,
    Gs1 = 9,
    SerialNumber = 10
}

public enum BarcodeOperationType
{
    Receipt = 1,
    Delivery = 2,
    TransferShip = 3,
    TransferReceive = 4,
    StockIn = 5,
    StockOut = 6,
    Adjustment = 7,
    CycleCount = 8
}

public enum BarcodeSessionStatus
{
    Draft = 1,
    Ready = 2,
    Applied = 3,
    Cancelled = 4
}

public enum BarcodeLineStatus
{
    Accepted = 1,
    Warning = 2,
    Rejected = 3
}
