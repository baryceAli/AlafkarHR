using SharedWithUI.SalesOrder.Enums;

namespace SharedWithUI.SalesOrder.Dtos;

public class SalesSettingsDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public SalesInvoicingPolicy InvoicingPolicy { get; set; } = SalesInvoicingPolicy.InvoiceDeliveredQuantity;
}
