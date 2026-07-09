namespace Sales.Sales.Features.GetSalesDashboard;

public record GetSalesDashboardQuery(Guid CompanyId) : IQuery<GetSalesDashboardResult>;
public record GetSalesDashboardResult(SalesDashboardDto Dashboard);

public class GetSalesDashboardHandler(SalesOrderDbContext dbContext)
    : IQueryHandler<GetSalesDashboardQuery, GetSalesDashboardResult>
{
    public async Task<GetSalesDashboardResult> Handle(GetSalesDashboardQuery request, CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;
        var soon = today.AddDays(7);
        var soonExclusive = soon.AddDays(1);
        var query = dbContext.SalesOrders.AsNoTracking().Where(x => x.CompanyId == request.CompanyId);
        var quotations = dbContext.SalesQuotations.AsNoTracking().Where(x => x.CompanyId == request.CompanyId);
        var returns = dbContext.SalesReturns.AsNoTracking().Where(x => x.CompanyId == request.CompanyId);
        var quotationCount = await quotations.CountAsync(cancellationToken);
        var convertedQuotationCount = await quotations.CountAsync(x => x.Status == SalesQuotationStatus.ConvertedToSalesOrder, cancellationToken);
        var dashboard = new SalesDashboardDto
        {
            DraftOrders = await query.CountAsync(x => x.Status == SalesOrderStatus.Draft, cancellationToken),
            ConfirmedOrders = await query.CountAsync(x => x.Status == SalesOrderStatus.Confirmed, cancellationToken),
            DraftQuotations = await quotations.CountAsync(x => x.Status == SalesQuotationStatus.Draft, cancellationToken),
            SentQuotations = await quotations.CountAsync(x => x.Status == SalesQuotationStatus.Sent, cancellationToken),
            ConvertedQuotations = convertedQuotationCount,
            QuotationsExpiringSoon = await quotations.CountAsync(x =>
                x.ValidUntil.HasValue
                && x.ValidUntil.Value >= today
                && x.ValidUntil.Value < soonExclusive
                && (x.Status == SalesQuotationStatus.Draft || x.Status == SalesQuotationStatus.Sent),
                cancellationToken),
            ExpiredQuotations = await quotations.CountAsync(x =>
                x.Status == SalesQuotationStatus.Expired
                || (x.ValidUntil.HasValue
                    && x.ValidUntil.Value < today
                    && (x.Status == SalesQuotationStatus.Draft || x.Status == SalesQuotationStatus.Sent)),
                cancellationToken),
            OptionalLineQuotations = await quotations.CountAsync(x => x.Lines.Any(line => line.IsOptional), cancellationToken),
            OptionalLineAdoptions = await quotations.CountAsync(x => x.Lines.Any(line => line.IsOptional && line.Quantity > 0), cancellationToken),
            DownPaymentQuotations = await quotations.CountAsync(x => x.DownPaymentAmount > 0 || x.DownPaymentPercent > 0, cancellationToken),
            DownPaymentValue = await quotations.SumAsync(x => x.DownPaymentAmount + (x.TotalAmount * x.DownPaymentPercent / 100m), cancellationToken),
            ProFormaQuotations = await quotations.CountAsync(x => x.IsProForma, cancellationToken),
            OpenQuotationValue = await quotations
                .Where(x => x.Status == SalesQuotationStatus.Draft || x.Status == SalesQuotationStatus.Sent)
                .SumAsync(x => x.TotalAmount, cancellationToken),
            QuotationConversionRate = quotationCount == 0 ? 0m : (decimal)convertedQuotationCount / quotationCount * 100m,
            DeliveredOrders = await query.CountAsync(x => x.Status == SalesOrderStatus.Delivered || x.Status == SalesOrderStatus.PartiallyDelivered, cancellationToken),
            InvoicedOrders = await query.CountAsync(x => x.Status == SalesOrderStatus.Invoiced || x.Status == SalesOrderStatus.PartiallyInvoiced, cancellationToken),
            CompletedOrders = await query.CountAsync(x => x.Status == SalesOrderStatus.Completed, cancellationToken),
            DeliveryBacklogOrders = await query.CountAsync(x => x.Status != SalesOrderStatus.Cancelled && x.Status != SalesOrderStatus.Completed && x.Lines.Any(l => l.DeliveredQuantity < l.Quantity), cancellationToken),
            InvoicedValue = await query.Where(x => x.Status == SalesOrderStatus.Invoiced || x.Status == SalesOrderStatus.PartiallyInvoiced || x.Status == SalesOrderStatus.Completed).SumAsync(x => x.TotalAmount, cancellationToken),
            ReturnedValue = await returns.Where(x => x.Status == SalesReturnStatus.Posted).SumAsync(x => x.TotalAmount, cancellationToken),
            OpenOrderValue = await query.Where(x => x.Status != SalesOrderStatus.Completed && x.Status != SalesOrderStatus.Cancelled).SumAsync(x => x.TotalAmount, cancellationToken),
            CompletedOrderValue = await query.Where(x => x.Status == SalesOrderStatus.Completed).SumAsync(x => x.TotalAmount, cancellationToken),
            TopCustomers = await query
                .GroupBy(x => x.CustomerId)
                .Select(x => new SalesDashboardBreakdownDto { Id = x.Key, Name = x.Key.ToString(), Value = x.Sum(o => o.TotalAmount), Quantity = x.Count() })
                .OrderByDescending(x => x.Value)
                .Take(5)
                .ToListAsync(cancellationToken),
            TopProducts = await query
                .SelectMany(x => x.Lines)
                .GroupBy(x => new { x.ProductSkuId, x.ProductNameEng, x.ProductName })
                .Select(x => new SalesDashboardBreakdownDto { Id = x.Key.ProductSkuId, Name = x.Key.ProductNameEng ?? x.Key.ProductName, Value = x.Sum(l => l.TotalAmount), Quantity = x.Sum(l => l.Quantity) })
                .OrderByDescending(x => x.Value)
                .Take(5)
                .ToListAsync(cancellationToken),
            SalespersonPerformance = await query
                .GroupBy(x => x.SalespersonId ?? string.Empty)
                .Select(x => new SalespersonPerformanceDto { SalespersonId = x.Key, Orders = x.Count(), OrderValue = x.Sum(o => o.TotalAmount) })
                .OrderByDescending(x => x.OrderValue)
                .Take(5)
                .ToListAsync(cancellationToken)
        };

        var quotationPerformance = await quotations
            .GroupBy(x => x.SalespersonId ?? string.Empty)
            .Select(x => new { SalespersonId = x.Key, Quotations = x.Count(), QuotationValue = x.Sum(q => q.TotalAmount) })
            .ToListAsync(cancellationToken);

        foreach (var performance in dashboard.SalespersonPerformance)
        {
            var quotation = quotationPerformance.FirstOrDefault(x => x.SalespersonId == performance.SalespersonId);
            if (quotation is not null)
            {
                performance.Quotations = quotation.Quotations;
                performance.QuotationValue = quotation.QuotationValue;
            }
        }

        return new GetSalesDashboardResult(dashboard);
    }
}
