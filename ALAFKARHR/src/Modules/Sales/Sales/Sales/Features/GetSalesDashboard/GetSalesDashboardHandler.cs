namespace Sales.Sales.Features.GetSalesDashboard;

public record GetSalesDashboardQuery(Guid CompanyId) : IQuery<GetSalesDashboardResult>;
public record GetSalesDashboardResult(SalesDashboardDto Dashboard);

public class GetSalesDashboardHandler(SalesOrderDbContext dbContext)
    : IQueryHandler<GetSalesDashboardQuery, GetSalesDashboardResult>
{
    public async Task<GetSalesDashboardResult> Handle(GetSalesDashboardQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.SalesOrders.AsNoTracking().Where(x => x.CompanyId == request.CompanyId);
        var dashboard = new SalesDashboardDto
        {
            DraftOrders = await query.CountAsync(x => x.Status == SalesOrderStatus.Draft, cancellationToken),
            ConfirmedOrders = await query.CountAsync(x => x.Status == SalesOrderStatus.Confirmed, cancellationToken),
            DeliveredOrders = await query.CountAsync(x => x.Status == SalesOrderStatus.Delivered || x.Status == SalesOrderStatus.PartiallyDelivered, cancellationToken),
            InvoicedOrders = await query.CountAsync(x => x.Status == SalesOrderStatus.Invoiced || x.Status == SalesOrderStatus.PartiallyInvoiced, cancellationToken),
            CompletedOrders = await query.CountAsync(x => x.Status == SalesOrderStatus.Completed, cancellationToken),
            OpenOrderValue = await query.Where(x => x.Status != SalesOrderStatus.Completed && x.Status != SalesOrderStatus.Cancelled).SumAsync(x => x.TotalAmount, cancellationToken),
            CompletedOrderValue = await query.Where(x => x.Status == SalesOrderStatus.Completed).SumAsync(x => x.TotalAmount, cancellationToken)
        };

        return new GetSalesDashboardResult(dashboard);
    }
}
