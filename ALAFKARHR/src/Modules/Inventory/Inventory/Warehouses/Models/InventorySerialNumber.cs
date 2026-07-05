namespace Inventory.Warehouses.Models;

public class InventorySerialNumber : Entity<Guid>
{
    private InventorySerialNumber() { }

    public Guid CompanyId { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid ProductSkuId { get; private set; }
    public string SerialNumber { get; private set; } = string.Empty;
    public Guid? BatchId { get; private set; }
    public Guid? WarehouseId { get; private set; }
    public Guid? WarehouseLocationId { get; private set; }
    public InventorySerialStatus Status { get; private set; } = InventorySerialStatus.Available;
    public Guid? SourceDocumentId { get; private set; }
    public Guid? SourceDocumentLineId { get; private set; }
    public Guid? LastStockMovementId { get; private set; }
    public DateTime? LastMovementAt { get; private set; }

    public static InventorySerialNumber Create(
        Guid companyId,
        Guid productId,
        Guid productSkuId,
        string serialNumber,
        Guid? batchId,
        Guid? warehouseId,
        Guid? warehouseLocationId,
        Guid? sourceDocumentId,
        Guid? sourceDocumentLineId,
        Guid stockMovementId,
        string createdBy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(serialNumber);
        ArgumentException.ThrowIfNullOrWhiteSpace(createdBy);

        return new InventorySerialNumber
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            ProductId = productId,
            ProductSkuId = productSkuId,
            SerialNumber = Normalize(serialNumber),
            BatchId = batchId,
            WarehouseId = warehouseId,
            WarehouseLocationId = warehouseLocationId,
            Status = InventorySerialStatus.Available,
            SourceDocumentId = sourceDocumentId,
            SourceDocumentLineId = sourceDocumentLineId,
            LastStockMovementId = stockMovementId,
            LastMovementAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void Receive(Guid? batchId, Guid warehouseId, Guid? warehouseLocationId, Guid stockMovementId, string modifiedBy)
        => Move(batchId, warehouseId, warehouseLocationId, InventorySerialStatus.Available, stockMovementId, modifiedBy);

    public void Reserve(Guid stockMovementId, string modifiedBy)
        => SetStatus(InventorySerialStatus.Reserved, stockMovementId, modifiedBy);

    public void Release(Guid stockMovementId, string modifiedBy)
        => SetStatus(InventorySerialStatus.Available, stockMovementId, modifiedBy);

    public void Consume(Guid stockMovementId, string modifiedBy)
        => Move(BatchId, null, null, InventorySerialStatus.Consumed, stockMovementId, modifiedBy);

    public void Scrap(Guid stockMovementId, string modifiedBy)
        => Move(BatchId, null, null, InventorySerialStatus.Scrapped, stockMovementId, modifiedBy);

    public void ReturnToStock(Guid? batchId, Guid warehouseId, Guid? warehouseLocationId, Guid stockMovementId, string modifiedBy)
        => Move(batchId, warehouseId, warehouseLocationId, InventorySerialStatus.Returned, stockMovementId, modifiedBy);

    public void Move(Guid? batchId, Guid? warehouseId, Guid? warehouseLocationId, InventorySerialStatus status, Guid stockMovementId, string modifiedBy)
    {
        BatchId = batchId;
        WarehouseId = warehouseId;
        WarehouseLocationId = warehouseLocationId;
        SetStatus(status, stockMovementId, modifiedBy);
    }

    private void SetStatus(InventorySerialStatus status, Guid stockMovementId, string modifiedBy)
    {
        Status = status;
        LastStockMovementId = stockMovementId;
        LastMovementAt = DateTime.UtcNow;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public static string Normalize(string serialNumber)
        => serialNumber.Trim().ToUpperInvariant();
}

public class StockMovementSerial : Entity<Guid>
{
    private StockMovementSerial() { }

    public Guid StockMovementId { get; private set; }
    public Guid InventorySerialNumberId { get; private set; }
    public string SerialNumber { get; private set; } = string.Empty;
    public InventorySerialStatus StatusAfterMovement { get; private set; }

    public static StockMovementSerial Create(
        Guid stockMovementId,
        Guid inventorySerialNumberId,
        string serialNumber,
        InventorySerialStatus statusAfterMovement,
        string createdBy)
    {
        return new StockMovementSerial
        {
            Id = Guid.NewGuid(),
            StockMovementId = stockMovementId,
            InventorySerialNumberId = inventorySerialNumberId,
            SerialNumber = InventorySerialNumber.Normalize(serialNumber),
            StatusAfterMovement = statusAfterMovement,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }
}
