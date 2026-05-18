using Shared.DDD;

namespace Inventory.Warehouses.Models;

public class TransferItem:Entity<Guid>
{
    public Guid ProductId { get; private set; }
    public Guid ProductSkuId { get; private set; }
    public Guid BatchId { get; private set; }
    //public Guid WarehouseId { get; set; }
    public decimal Quantity { get; private set; }
    public decimal ReceivedQuantity { get; private set; }
    public bool IsCompleted => Quantity >= ReceivedQuantity;

    public TransferItem(){}
    internal TransferItem(Guid productId,
        Guid productSkuId,
        Guid batchId,
        //Guid warehouseId,
        decimal quantity,
        //decimal receivedQuantity,
        //bool isCompleted,
        string createdBy)
    {
        if (quantity <= 0)
            throw new InvalidOperationException(
                "Quantity must be greater than zero");
        ProductId = productId;
        ProductSkuId = productSkuId;
        BatchId = batchId;
        //WarehouseId = warehouseId,
        Quantity = quantity;
        ReceivedQuantity = 0;
        //IsCompleted = receivedQuantity==quantity,
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }
    public static TransferItem Create(
        //Guid id,
        Guid productId,
        Guid productSkuId,
        Guid batchId,
        //Guid warehouseId,
        decimal quantity,
        //decimal receivedQuantity,
        //bool isCompleted,
        string createdBy
        )
    {
        if (quantity <= 0)
            throw new InvalidOperationException(
                "Quantity must be greater than zero");
        return new TransferItem
        {
            //Id = id,
            ProductId = productId,
            ProductSkuId = productSkuId,
            BatchId = batchId,
            //WarehouseId = warehouseId,
            Quantity = quantity,
            ReceivedQuantity =0,
            //IsCompleted = receivedQuantity==quantity,
            CreatedAt= DateTime.UtcNow,
            CreatedBy= createdBy
        };
    }

    public void Receive(decimal quantity, string user)
    {
        if (quantity <= 0)
            throw new InvalidOperationException(
                "Quantity must be greater than zero");

        if (IsCompleted)
            throw new InvalidOperationException(
                "Item already completed");

        if (ReceivedQuantity + quantity > Quantity)
            throw new InvalidOperationException(
                "Cannot receive more than shipped quantity");

        ReceivedQuantity += quantity;

        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = user;
    }
}