using Inventory.Warehouses.Enums;
using Shared.DDD;

namespace Inventory.Warehouses.Models;

public class WarehouseTransfer : Aggregate<Guid>
{
    public Guid SourceWarehouseId { get; private set; }
    public Guid DestinationWarehouseId { get; private set; }

    public TransferStatus Status { get; private set; }

    private readonly List<TransferItem> _items = new();
    public IReadOnlyCollection<TransferItem> Items => _items;

    public DateTime CreatedAt { get; private set; }
    public DateTime? ShippedAt { get; private set; }
    public DateTime? ReceivedAt { get; private set; }

    //public void Create(...) { }

    public void Ship(string user)
    {
        if (Status != TransferStatus.Pending)
            throw new Exception("Invalid state");

        Status = TransferStatus.Shipped;
        ShippedAt = DateTime.UtcNow;

        // 🔴 Create OUT movements
    }

    public void Receive(List<ReceiveItemDto> receivedItems, string user)
    {
        if (Status != TransferStatus.Shipped)
            throw new Exception("Not ready for receiving");

        // Validate quantities (allow partial)

        Status = TransferStatus.Completed;
        ReceivedAt = DateTime.UtcNow;

        // 🟢 Create IN movements
    }
}