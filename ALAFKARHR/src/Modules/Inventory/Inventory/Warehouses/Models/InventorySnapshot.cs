using Shared.DDD;
using System.ComponentModel.DataAnnotations;

namespace Inventory.Warehouses.Models;

public class InventorySnapshot : Entity<Guid>
{
    public Guid ProductId { get; private set; }
    public Guid ProductSkuId { get; private set; }
    public Guid WarehouseId { get; private set; }
    public Guid BatchId { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal ReservedQuantity { get; private set; }
    [Timestamp]
    public byte[] RowVersion { get; private set; }
    //public DateTime LastUpdated { get; private set; }

    private InventorySnapshot() { }
    public static InventorySnapshot Create(Guid id,
        Guid productId,
        Guid productSkuId,
        Guid warehouseId,
        Guid batchId,
        decimal quantity,
        decimal reservedQuantity, string createdBy)
    {
        return new InventorySnapshot()
        {

            Id = id,
            ProductId = productId,
            ProductSkuId = productSkuId,
            WarehouseId = warehouseId,
            BatchId = batchId,
            Quantity = quantity,
            ReservedQuantity = reservedQuantity,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Remove(string deletedBy) 
    {
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }
    public void IncreaseQuantity(decimal quantity, string modifiedBy)
    {
        Quantity += quantity;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }
    public void DecreaseQuantity(decimal quantity, string modifiedBy)
    {
        Quantity -= quantity;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }
    public void Reserve(decimal quantity, string modifiedBy)
    {
        if (Quantity < quantity)
            throw new Exception("Invalid Quantity");
        Quantity -= quantity;
        ReservedQuantity += quantity;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }
    public void Release(decimal quantity, string modifiedBy) 
    {
        if (ReservedQuantity < quantity)
            throw new Exception("Invalid Quantity");

        Quantity += quantity;
        ReservedQuantity -= quantity;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    //UNIQUE(ProductId, WarehouseId, BatchId)

}
