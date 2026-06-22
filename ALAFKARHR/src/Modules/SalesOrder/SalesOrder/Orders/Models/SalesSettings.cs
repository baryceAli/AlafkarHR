using Shared.DDD;

namespace SalesOrder.Orders.Models;

public class SalesSettings : Aggregate<Guid>
{
    private SalesSettings() { }

    public Guid CompanyId { get; private set; }
    public SalesInvoicingPolicy InvoicingPolicy { get; private set; }

    public static SalesSettings Create(Guid companyId, SalesInvoicingPolicy invoicingPolicy, string userId) =>
        new()
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            InvoicingPolicy = invoicingPolicy,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

    public void Update(SalesInvoicingPolicy invoicingPolicy, string userId)
    {
        InvoicingPolicy = invoicingPolicy;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }
}
