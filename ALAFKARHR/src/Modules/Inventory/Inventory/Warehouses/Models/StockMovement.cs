using Shared.DDD;

namespace Inventory.Warehouses.Models;

public class StockMovement : Entity<Guid>
{
    public Guid ProductId { get; private set; }
    public Guid ProductSkuId { get; private set; }
    public Guid WarehouseId { get; private set; }
    public Guid BatchId { get; private set; }
    public MovementType MovementType { get; private set; }

    public string ReferenceNumber { get; private set; }
    public string SourceDocumentType { get; private set; }

    public decimal QuantityBefore { get; private set; }

    public decimal QuantityAfter { get; private set; }
    //public decimal Quantity { get; private set; }
    public decimal ReservedBefore { get; private set; }
    public decimal ReservedAfter { get; private set; }
    public decimal UnitCost { get; set; }
    public decimal TotalCost { get; set; }
    public Guid Currency { get; set; }

    public string Notes { get; private set; } = string.Empty;
    public MovementDirection MovementDirection { get; private set; }
    //public MovementCategory MovementCategory { get; private set; }






    private StockMovement() { }

    public static StockMovement Create(
        Guid id,
        Guid warehouseId,
        Guid batchId,
        Guid productId,
        Guid productSkuId,
        decimal quantityBefore,
        decimal quantityAfter,
        //decimal quantity,
        decimal reservedBefore,
        decimal reservedAfter,
        decimal unitCost,
        decimal totalCost,
        Guid currency,
        string referenceNumber,
        string sourceDocumentType,
        //DateTime movementDate,
        MovementType movementType,
        MovementDirection movementDirection,
        //MovementCategory movementCategory,
        string createdBy,
        string notes = "")
    {
        ArgumentNullException.ThrowIfNull(productSkuId);
        ArgumentNullException.ThrowIfNull(warehouseId);
        ArgumentNullException.ThrowIfNull(batchId);
        ArgumentOutOfRangeException.ThrowIfNegative(quantityBefore);
        ArgumentOutOfRangeException.ThrowIfNegative(quantityAfter);
        ArgumentException.ThrowIfNullOrWhiteSpace(createdBy);

        return new StockMovement
        {
            Id = id,
            ProductId = productId,
            ProductSkuId = productSkuId,
            WarehouseId = warehouseId,
            BatchId = batchId,
            MovementType = movementType,

            QuantityBefore = quantityBefore,
            QuantityAfter = quantityAfter,
            //Quantity = quantity,
            ReservedBefore = reservedBefore,
            ReservedAfter = reservedAfter,

            UnitCost = unitCost,
            TotalCost = totalCost,
            Currency = currency,


            ReferenceNumber = referenceNumber,
            SourceDocumentType = sourceDocumentType,
            //MovementDate = movementDate,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy,
            MovementDirection = movementDirection,
            //MovementCategory = movementCategory,
            Notes = notes
        };
    }
}
