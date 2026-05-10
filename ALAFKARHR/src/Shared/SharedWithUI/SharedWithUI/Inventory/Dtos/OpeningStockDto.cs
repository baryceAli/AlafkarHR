namespace SharedWithUI.Inventory.Dtos;

public class OpeningStockDto
{
    public Guid WarehouseId { get; set; }
    public Guid ProductId { get; set; }
    public Guid ProductSkuId { get; set; }
    public Guid BatchId { get; set; }
    public decimal Quantity { get; set; }


}
