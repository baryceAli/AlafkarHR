using Shared.Pagination;

namespace Sales.Sales.Features.GetSalesOrdersByCompany;

public record GetSalesOrdersByCompanyQuery(Guid CompanyId, PaginationRequest PaginationRequest)
    : IQuery<GetSalesOrdersByCompanyResult>;
public record GetSalesOrdersByCompanyResult(PaginatedResult<SalesOrderDto> SalesOrders);

public class GetSalesOrdersByCompanyHandler(SalesOrderDbContext dbContext)
    : IQueryHandler<GetSalesOrdersByCompanyQuery, GetSalesOrdersByCompanyResult>
{
    public async Task<GetSalesOrdersByCompanyResult> Handle(GetSalesOrdersByCompanyQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.SalesOrders.Include(x => x.Lines).AsNoTracking().Where(x => x.CompanyId == request.CompanyId);
        if (!string.IsNullOrWhiteSpace(request.PaginationRequest.SearchText))
        {
            var search = request.PaginationRequest.SearchText;
            query = query.Where(x => x.Number.Contains(search));
        }

        var count = await query.LongCountAsync(cancellationToken);
        var orders = await query
            .OrderByDescending(x => x.OrderDate)
            .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
            .Take(request.PaginationRequest.PageSize)
            .ToListAsync(cancellationToken);

        return new GetSalesOrdersByCompanyResult(
            new PaginatedResult<SalesOrderDto>(
                request.PaginationRequest.PageIndex,
                request.PaginationRequest.PageSize,
                count,
                orders.Adapt<List<SalesOrderDto>>()));
    }
}
