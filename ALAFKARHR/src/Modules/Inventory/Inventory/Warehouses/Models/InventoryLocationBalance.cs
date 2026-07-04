namespace Inventory.Warehouses.Models;

public class InventoryLocationBalance : Aggregate<Guid>
{
    private InventoryLocationBalance() { }

    public Guid CompanyId { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid ProductSkuId { get; private set; }
    public Guid WarehouseId { get; private set; }
    public Guid WarehouseLocationId { get; private set; }
    public Guid BatchId { get; private set; }
    public Batch Batch { get; private set; } = null!;
    public decimal Quantity { get; private set; }
    public decimal ReservedQuantity { get; private set; }
    public decimal AvailableQuantity => Quantity - ReservedQuantity;
    public byte[] RowVersion { get; private set; } = [];

    public static InventoryLocationBalance Create(
        Guid companyId,
        Guid productId,
        Guid productSkuId,
        Guid warehouseId,
        Guid warehouseLocationId,
        Guid batchId,
        string userId)
    {
        if (companyId == Guid.Empty) throw new ArgumentNullException(nameof(companyId));
        if (productId == Guid.Empty) throw new ArgumentNullException(nameof(productId));
        if (productSkuId == Guid.Empty) throw new ArgumentNullException(nameof(productSkuId));
        if (warehouseId == Guid.Empty) throw new ArgumentNullException(nameof(warehouseId));
        if (warehouseLocationId == Guid.Empty) throw new ArgumentNullException(nameof(warehouseLocationId));
        if (batchId == Guid.Empty) throw new ArgumentNullException(nameof(batchId));

        return new InventoryLocationBalance
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            ProductId = productId,
            ProductSkuId = productSkuId,
            WarehouseId = warehouseId,
            WarehouseLocationId = warehouseLocationId,
            BatchId = batchId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };
    }

    public void Increase(decimal quantity, string userId)
    {
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));
        Quantity += quantity;
        Touch(userId);
    }

    public void Decrease(decimal quantity, string userId)
    {
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));
        if (quantity > AvailableQuantity)
            throw new InvalidOperationException("Insufficient available stock in the selected location.");
        Quantity -= quantity;
        Touch(userId);
    }

    public void Reserve(decimal quantity, string userId)
    {
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));
        if (quantity > AvailableQuantity)
            throw new InvalidOperationException("Insufficient available stock in the selected location.");
        ReservedQuantity += quantity;
        Touch(userId);
    }

    public void Release(decimal quantity, string userId)
    {
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));
        if (quantity > ReservedQuantity)
            throw new InvalidOperationException("Insufficient reserved stock in the selected location.");
        ReservedQuantity -= quantity;
        Touch(userId);
    }

    public void ConsumeReserved(decimal quantity, string userId)
    {
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));
        if (quantity > ReservedQuantity)
            throw new InvalidOperationException("Insufficient reserved stock in the selected location.");
        if (quantity > Quantity)
            throw new InvalidOperationException("Insufficient stock in the selected location.");
        ReservedQuantity -= quantity;
        Quantity -= quantity;
        Touch(userId);
    }

    public void AdjustTo(decimal countedQuantity, string userId)
    {
        if (countedQuantity < 0) throw new ArgumentOutOfRangeException(nameof(countedQuantity));
        if (countedQuantity < ReservedQuantity)
            throw new InvalidOperationException("Counted quantity cannot be lower than reserved quantity.");
        Quantity = countedQuantity;
        Touch(userId);
    }

    private void Touch(string userId)
    {
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }
}
