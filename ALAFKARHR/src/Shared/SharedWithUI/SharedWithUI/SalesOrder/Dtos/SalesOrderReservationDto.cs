namespace SharedWithUI.SalesOrder.Dtos;

public class SalesOrderReservationRequestDto
{
    public Guid WarehouseId { get; set; }
    public List<SalesOrderReservationLineDto> Lines { get; set; } = [];
}

public class SalesOrderReservationLineDto
{
    public Guid SalesOrderLineId { get; set; }
    public decimal Quantity { get; set; }
}
