namespace SharedWithUI.Inventory.Dtos;

public class TransferItemDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get;  set; }
    public Guid ProductSkuId { get;  set; }
    public Guid BatchId { get; set; }
    //public Guid WarehouseId { get; set; }
    public decimal Quantity { get; set; }
    public decimal ReceivedQuantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalCost => Quantity * UnitCost;
    public Guid CurrencyId { get; set; }
    public bool IsCompleted { get; set; }

}
