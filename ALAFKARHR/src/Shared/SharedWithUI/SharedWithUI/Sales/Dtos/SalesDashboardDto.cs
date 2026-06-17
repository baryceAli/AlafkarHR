namespace SharedWithUI.Sales.Dtos;

public class SalesDashboardDto
{
    public int DraftOrders { get; set; }
    public int ConfirmedOrders { get; set; }
    public int DeliveredOrders { get; set; }
    public int InvoicedOrders { get; set; }
    public int CompletedOrders { get; set; }
    public decimal OpenOrderValue { get; set; }
    public decimal CompletedOrderValue { get; set; }
}
