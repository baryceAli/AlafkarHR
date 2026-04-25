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
}