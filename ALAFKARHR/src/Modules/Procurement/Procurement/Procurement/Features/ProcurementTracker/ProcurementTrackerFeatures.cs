namespace Procurement.Procurement.Features;

public record GetProcurementTrackerQuery(Guid CompanyId) : IQuery<GetProcurementTrackerResult>;
public record GetProcurementTrackerResult(IReadOnlyCollection<ProcurementTrackerRowDto> Rows);

public class GetProcurementTrackerHandler(ProcurementDbContext dbContext)
    : IQueryHandler<GetProcurementTrackerQuery, GetProcurementTrackerResult>
{
    public async Task<GetProcurementTrackerResult> Handle(GetProcurementTrackerQuery request, CancellationToken cancellationToken)
    {
        var documents = await dbContext.ProcurementDocuments.AsNoTracking()
            .Include(x => x.Lines)
            .Where(x => x.CompanyId == request.CompanyId)
            .OrderByDescending(x => x.DocumentDate)
            .ToListAsync(cancellationToken);

        var receiptsBySource = documents
            .Where(x => x.Kind == ProcurementDocumentKind.GoodsReceipt && x.Status == PostedDocumentStatus.Posted.ToString() && x.SourceDocumentId.HasValue)
            .GroupBy(x => x.SourceDocumentId!.Value)
            .ToDictionary(x => x.Key, x => x.SelectMany(d => d.Lines).Sum(l => l.Quantity));

        var invoicesBySource = documents
            .Where(x => x.Kind == ProcurementDocumentKind.SupplierInvoice && x.Status != SupplierInvoiceStatus.Cancelled.ToString() && x.SourceDocumentId.HasValue)
            .GroupBy(x => x.SourceDocumentId!.Value)
            .ToDictionary(x => x.Key, x => x.SelectMany(d => d.Lines).Sum(l => l.Quantity));

        var rows = documents.Select(document =>
        {
            var ordered = document.Lines.Sum(x => x.Quantity);
            receiptsBySource.TryGetValue(document.Id, out var received);
            invoicesBySource.TryGetValue(document.Id, out var invoiced);
            return new ProcurementTrackerRowDto
            {
                Id = document.Id,
                Kind = document.Kind,
                Number = document.Number,
                Status = document.Status,
                DocumentDate = document.DocumentDate,
                SourceDocumentId = document.SourceDocumentId,
                SourceDocumentNumber = document.SourceDocumentNumber,
                SupplierId = document.SupplierId,
                SupplierName = document.SupplierName,
                OrderedQuantity = ordered,
                ReceivedQuantity = received,
                InvoicedQuantity = invoiced,
                OpenQuantity = Math.Max(ordered - Math.Max(received, invoiced), 0),
                AgeDays = (DateTime.UtcNow.Date - document.DocumentDate.Date).Days
            };
        }).ToList();

        return new GetProcurementTrackerResult(rows);
    }
}
