using Shared.DDD;

namespace Inventory.Warehouses.Models;

public class TransferItem:Entity<Guid>
{
    public Guid ProductId { get; private set; }
    public Guid ProductSkuId { get; private set; }
    public Guid BatchId { get; private set; }
    public Guid WarehouseId { get; set; }
    public decimal Quantity { get; private set; }
    public decimal? ReceivedQuantity { get; private set; }
    public bool IsCompleted { get; private set; } = false;

    public TransferItem(){}

    public static TransferItem Create(
        //Guid id,
        Guid productId,
        Guid productSkuId,
        Guid batchId,
        Guid warehouseId,
        decimal quantity,
        decimal? receivedQuantity,
        //bool isCompleted,
        string createdBy
        )
    {
        return new TransferItem
        {
            //Id = id,
            ProductId = productId,
            ProductSkuId = productSkuId,
            BatchId = batchId,
            WarehouseId = warehouseId,
            Quantity = quantity,
            ReceivedQuantity = receivedQuantity,
            IsCompleted = receivedQuantity==quantity,
            CreatedAt= DateTime.UtcNow,
            CreatedBy= createdBy
        };
    }

    public void Receive(decimal quantity, string user)
    {
        ReceivedQuantity += quantity;
        ModifiedAt= DateTime.UtcNow;
        IsCompleted = ReceivedQuantity == Quantity ;
        ModifiedBy = user;
    }
}