namespace SharedWithUI.Inventory.Dtos;

public class TransferItemDto
{
    public Guid ProductId { get;  set; }
    public Guid ProductSkuId { get;  set; }
    public Guid BatchId { get; set; }
    //public Guid WarehouseId { get; set; }
    public decimal Quantity { get; set; }
    public decimal ReceivedQuantity { get; set; }
    public bool IsCompleted { get; set; }

}
