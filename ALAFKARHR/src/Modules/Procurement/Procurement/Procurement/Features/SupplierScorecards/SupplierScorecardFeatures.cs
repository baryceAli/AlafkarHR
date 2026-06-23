namespace Procurement.Procurement.Features;

public record GetSupplierScorecardQuery(Guid CompanyId) : IQuery<GetSupplierScorecardResult>;
public record GetSupplierScorecardResult(IReadOnlyCollection<SupplierScorecardRowDto> Rows);

public class GetSupplierScorecardHandler(ProcurementDbContext dbContext)
    : IQueryHandler<GetSupplierScorecardQuery, GetSupplierScorecardResult>
{
    public async Task<GetSupplierScorecardResult> Handle(GetSupplierScorecardQuery request, CancellationToken cancellationToken)
    {
        var documents = await dbContext.ProcurementDocuments.AsNoTracking()
            .Include(x => x.Lines)
            .Where(x => x.CompanyId == request.CompanyId && x.SupplierId.HasValue)
            .ToListAsync(cancellationToken);

        var rows = documents.GroupBy(x => new { SupplierId = x.SupplierId!.Value, x.SupplierName })
            .Select(group =>
            {
                var ordered = group.Where(x => x.Kind == ProcurementDocumentKind.PurchaseOrder).SelectMany(x => x.Lines).Sum(x => x.Quantity);
                var received = group.Where(x => x.Kind == ProcurementDocumentKind.GoodsReceipt && x.Status == PostedDocumentStatus.Posted.ToString()).SelectMany(x => x.Lines).Sum(x => x.Quantity);
                var invoiced = group.Where(x => x.Kind == ProcurementDocumentKind.SupplierInvoice && x.Status != SupplierInvoiceStatus.Cancelled.ToString()).SelectMany(x => x.Lines).Sum(x => x.Quantity);
                return new SupplierScorecardRowDto
                {
                    SupplierId = group.Key.SupplierId,
                    SupplierName = group.Key.SupplierName ?? "-",
                    PurchaseOrders = group.Count(x => x.Kind == ProcurementDocumentKind.PurchaseOrder),
                    GoodsReceipts = group.Count(x => x.Kind == ProcurementDocumentKind.GoodsReceipt),
                    SupplierInvoices = group.Count(x => x.Kind == ProcurementDocumentKind.SupplierInvoice),
                    OrderedQuantity = ordered,
                    ReceivedQuantity = received,
                    InvoicedQuantity = invoiced,
                    ReceiptCompletionRate = ordered <= 0 ? 0 : Math.Round(received / ordered * 100m, 2),
                    InvoiceMatchRate = received <= 0 ? 0 : Math.Round(invoiced / received * 100m, 2)
                };
            })
            .OrderByDescending(x => x.OrderedQuantity)
            .ToList();

        return new GetSupplierScorecardResult(rows);
    }
}
