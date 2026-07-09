namespace SharedWithUI.Sales.Dtos;

public class SalesDashboardDto
{
    public int DraftOrders { get; set; }
    public int ConfirmedOrders { get; set; }
    public int DraftQuotations { get; set; }
    public int SentQuotations { get; set; }
    public int ConvertedQuotations { get; set; }
    public int QuotationsExpiringSoon { get; set; }
    public int ExpiredQuotations { get; set; }
    public int OptionalLineQuotations { get; set; }
    public int OptionalLineAdoptions { get; set; }
    public int DownPaymentQuotations { get; set; }
    public decimal DownPaymentValue { get; set; }
    public int ProFormaQuotations { get; set; }
    public decimal OpenQuotationValue { get; set; }
    public decimal QuotationConversionRate { get; set; }
    public int DeliveredOrders { get; set; }
    public int InvoicedOrders { get; set; }
    public int CompletedOrders { get; set; }
    public int DeliveryBacklogOrders { get; set; }
    public decimal InvoicedValue { get; set; }
    public decimal ReturnedValue { get; set; }
    public decimal OpenOrderValue { get; set; }
    public decimal CompletedOrderValue { get; set; }
    public List<SalesDashboardBreakdownDto> TopCustomers { get; set; } = [];
    public List<SalesDashboardBreakdownDto> TopProducts { get; set; } = [];
    public List<SalespersonPerformanceDto> SalespersonPerformance { get; set; } = [];
}

public class SalesDashboardBreakdownDto
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public decimal Quantity { get; set; }
}

public class SalespersonPerformanceDto
{
    public string SalespersonId { get; set; } = string.Empty;
    public int Quotations { get; set; }
    public int Orders { get; set; }
    public decimal QuotationValue { get; set; }
    public decimal OrderValue { get; set; }
}
