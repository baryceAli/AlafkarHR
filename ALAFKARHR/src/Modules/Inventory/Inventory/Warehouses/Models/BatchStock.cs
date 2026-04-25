using Shared.DDD;

namespace Inventory.Warehouses.Models;

public class BatchStock:Entity<Guid>
{
    public Guid BatchId { get; private set; }
    public Guid WarehouseId { get; set; }
    public decimal Quantity { get; private set; }
    public decimal ReservedQuantity { get; private set; }

    public decimal Available => Quantity - ReservedQuantity;

    private readonly List<BatchStock> _stocks = new ();
    public IReadOnlyList<BatchStock> Stocks => _stocks;
    private BatchStock(){}

    public static BatchStock Created(Guid id,
        Guid batchId,
        Guid warehouseId,
        //decimal quantity,
        //decimal reservedQuantity,
        string createdBy)
    {

        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(batchId);
        //ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
        //ArgumentOutOfRangeException.ThrowIfNegative(reservedQuantity);

        //if (quantity < 0 || reservedQuantity < 0 || reservedQuantity > quantity)
            //throw new InvalidOperationException("Invalid stock quantities");

        return new BatchStock
        {
            Id = id,
            BatchId = batchId,
            Quantity = 0,
            ReservedQuantity = 0,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
        
    }
    public void Update(decimal quantity,
        decimal reservedQuantity,
        string modifiedBy)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
        ArgumentOutOfRangeException.ThrowIfNegative(reservedQuantity);
        ArgumentException.ThrowIfNullOrWhiteSpace(modifiedBy);

        if (reservedQuantity > quantity)
        {
            throw new InvalidOperationException($"ReservedQuantity ({reservedQuantity}) should be equal to or less than quantity ({quantity})");
        }

        Quantity = quantity;
        ReservedQuantity = reservedQuantity;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void Remove(string deletedBy)
    {
        if (ReservedQuantity > 0)
            throw new InvalidOperationException("Cannot remove batch with reserved stock");
        DeletedAt =DateTime.UtcNow;
        DeletedBy= deletedBy;
    }

    // Adjust quantity
    public void Increase(decimal qty, string modifiedBy)
    {
        if (qty <= 0) throw new ArgumentOutOfRangeException(nameof(qty));
        Quantity += qty;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void Decrease(decimal qty, string modifiedBy)
    {
        if (qty <= 0) throw new ArgumentOutOfRangeException(nameof(qty));
        if (qty > Quantity) throw new InvalidOperationException("Insufficient stock");
        Quantity -= qty;
        if (ReservedQuantity > Quantity) ReservedQuantity = Quantity; // Safety
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    // Reserve stock
    public void Reserve(decimal qty, string modifiedBy)
    {
        if (qty <= 0) throw new ArgumentOutOfRangeException(nameof(qty));
        if (qty > Available) throw new InvalidOperationException("Insufficient available stock");
        ReservedQuantity += qty;
        Quantity -= qty;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    // Release reserved stock
    public void Release(decimal qty, string modifiedBy)
    {
        if (qty <= 0) throw new ArgumentOutOfRangeException(nameof(qty));
        if (qty > ReservedQuantity) throw new InvalidOperationException("Insufficient reserved stock");
        ReservedQuantity -= qty;
        Quantity += qty;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

}
