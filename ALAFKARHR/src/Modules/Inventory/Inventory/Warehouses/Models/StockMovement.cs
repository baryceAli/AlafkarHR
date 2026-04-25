using Inventory.Warehouses.Enums;
using Shared.DDD;

namespace Inventory.Warehouses.Models;

public class StockMovement : Entity<Guid>
{
    public Guid ProductId { get; private set; }
    public Guid ProductSkuId { get; private set; }
    public Guid WarehouseId { get; private set; }
    public Guid BatchId { get; private set; }
    public MovementType MovementType { get; private set; }
    public Guid ReferenceId { get; private set; }
    public decimal Quantity { get; private set; }
    public DateTime MovementDate { get; private set; }
    public string Notes { get; private set; } = string.Empty;
    public MovementDirection MovementDirection { get; set; }
    public MovementCategory MovementCategory { get; set; }
    private StockMovement() { }

    public static StockMovement Create(
        Guid id,
        Guid warehouseId,
        Guid batchId,
        Guid productId,
        Guid productSkuId,
        decimal quantity,
        Guid referenceId,
        DateTime movementDate,
        MovementType movementType,
        MovementDirection movementDirection,
        MovementCategory movementCategory,
        string createdBy,
        string notes = "")
    {
        ArgumentNullException.ThrowIfNull(productSkuId);
        ArgumentNullException.ThrowIfNull(warehouseId);
        ArgumentNullException.ThrowIfNull(batchId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
        ArgumentException.ThrowIfNullOrWhiteSpace(createdBy);

        return new StockMovement
        {
            Id = id,
            ProductSkuId = productSkuId,
            WarehouseId = warehouseId,
            BatchId = batchId,
            MovementType = movementType,
            Quantity = quantity,
            ReferenceId = referenceId,
            MovementDate = movementDate,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy,
            MovementDirection = movementDirection,
            MovementCategory = movementCategory,
            Notes = notes
        };
    }
}
