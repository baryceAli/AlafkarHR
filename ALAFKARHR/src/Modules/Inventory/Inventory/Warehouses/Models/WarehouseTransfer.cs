using Inventory.Warehouses.Enums;
using Microsoft.IdentityModel.Tokens;
using Shared.DDD;

public class WarehouseTransfer : Aggregate<Guid>
{
    public Guid SourceWarehouseId { get; private set; }

    public Guid DestinationWarehouseId { get; private set; }

    public TransferStatus Status { get; private set; }

    public DateTime? ShippedAt { get; private set; }

    public DateTime? ReceivedAt { get; private set; }

    //    TransferNumber
    //Notes
    //RequestedBy
    //ApprovedBy
    //Reason
    //ReferenceNumber
    //ExpectedDeliveryDate

    private readonly List<TransferItem> _items = new();

    public IReadOnlyCollection<TransferItem> Items =>
        _items.AsReadOnly();

    private WarehouseTransfer() { }

    public static WarehouseTransfer Create(
        Guid id,
        Guid sourceWarehouseId,
        Guid destinationWarehouseId,
        string createdBy)
    {
        if (sourceWarehouseId == destinationWarehouseId)
            throw new InvalidOperationException(
                "Source and destination cannot be the same");

        return new WarehouseTransfer
        {
            Id = id,
            SourceWarehouseId = sourceWarehouseId,
            DestinationWarehouseId = destinationWarehouseId,
            Status = TransferStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void AddItem(
        //Guid itemId,
        Guid productId,
        Guid productSkuId,
        Guid batchId,
        Guid warehouseId,
        decimal quantity,
        decimal? receivedQuantity,
        //bool isCompleted,
        string user)
    {
        EnsurePending();

        var existing = _items.FirstOrDefault(x =>
            x.ProductSkuId == productSkuId &&
            x.BatchId == batchId);

        if (existing != null)
            throw new InvalidOperationException(
                "Duplicate transfer item");

        var item = TransferItem.Create(
            //itemId,
            productId,
            productSkuId,
            batchId,
            warehouseId,
            quantity,
            receivedQuantity,
            //isCompleted,
            user);

        _items.Add(item);

        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = user;
    }

    public void RemoveItem(Guid itemId, string user)
    {
        EnsurePending();

        var item = FindItem(itemId);

        _items.Remove(item);

        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = user;
    }

    public void Ship(string user)
    {
        EnsurePending();

        if (!_items.Any())
            throw new InvalidOperationException(
                "Transfer has no items");

        Status = TransferStatus.Shipped;

        ShippedAt = DateTime.UtcNow;

        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = user;

        // Domain Event:
        // TransferShippedEvent
    }

    public void Receive(
    Guid itemId,
    decimal quantity,
    string user)
    {
        if (Status != TransferStatus.Shipped &&
            Status != TransferStatus.PartiallyReceived)
        {
            throw new InvalidOperationException(
                "Transfer not ready for receiving");
        }

        var item = FindItem(itemId);

        item.Receive(quantity, user);

        // All completed
        if (_items.All(x => x.IsCompleted))
        {
            Status = TransferStatus.Completed;
            ReceivedAt = DateTime.UtcNow;
        }
        // Some received but not completed
        else if (_items.Any(x => x.ReceivedQuantity != x.Quantity))
        {
            Status = TransferStatus.PartiallyReceived;
        }

        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = user;
    }
    public void Cancel(string user)
    {
        if (Status == TransferStatus.Completed)
            throw new InvalidOperationException(
                "Completed transfer cannot be cancelled");

        if (Status == TransferStatus.Cancelled)
            throw new InvalidOperationException(
                "Already cancelled");

        Status = TransferStatus.Cancelled;

        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = user;

        // Domain Event:
        // TransferCancelledEvent
    }

    private TransferItem FindItem(Guid itemId)
    {
        return _items.FirstOrDefault(x => x.Id == itemId)
            ?? throw new InvalidOperationException(
                $"Transfer item not found: {itemId}");
    }

    private void EnsurePending()
    {
        if (Status != TransferStatus.Pending)
            throw new InvalidOperationException(
                "Transfer is no longer editable");
    }
}