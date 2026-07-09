using Shared.Pagination;

namespace Sales.Sales.Features.GetSalesOrdersByCompany;

public record GetSalesOrdersByCompanyQuery(
    Guid CompanyId,
    PaginationRequest PaginationRequest,
    Guid? CustomerId = null,
    Guid? ProductId = null,
    Guid? ProductSkuId = null)
    : IQuery<GetSalesOrdersByCompanyResult>;
public record GetSalesOrdersByCompanyResult(PaginatedResult<SalesOrderDto> SalesOrders);
public record GetSalesOrderSmartLinksQuery(Guid CompanyId, Guid? CustomerId = null, Guid? ProductId = null, Guid? ProductSkuId = null)
    : IQuery<GetSalesOrderSmartLinksResult>;
public record GetSalesOrderSmartLinksResult(PartnerSmartLinkSummaryDto PartnerLinks, ProductSmartLinkSummaryDto ProductLinks);

public class GetSalesOrdersByCompanyHandler(SalesOrderDbContext dbContext)
    : IQueryHandler<GetSalesOrdersByCompanyQuery, GetSalesOrdersByCompanyResult>
{
    public async Task<GetSalesOrdersByCompanyResult> Handle(GetSalesOrdersByCompanyQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.SalesOrders.Include(x => x.Lines).AsNoTracking().Where(x => x.CompanyId == request.CompanyId);
        if (request.CustomerId.HasValue)
            query = query.Where(x => x.CustomerId == request.CustomerId.Value);

        if (request.ProductId.HasValue)
            query = query.Where(x => x.Lines.Any(line => line.ProductId == request.ProductId.Value));

        if (request.ProductSkuId.HasValue)
            query = query.Where(x => x.Lines.Any(line => line.ProductSkuId == request.ProductSkuId.Value));

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

public class GetSalesOrderSmartLinksHandler(SalesOrderDbContext dbContext)
    : IQueryHandler<GetSalesOrderSmartLinksQuery, GetSalesOrderSmartLinksResult>
{
    public async Task<GetSalesOrderSmartLinksResult> Handle(GetSalesOrderSmartLinksQuery request, CancellationToken cancellationToken)
    {
        var orders = dbContext.SalesOrders.AsNoTracking().Where(x => x.CompanyId == request.CompanyId);

        var partnerLinks = new PartnerSmartLinkSummaryDto();
        if (request.CustomerId.HasValue)
            partnerLinks.SalesOrders = await orders.CountAsync(x => x.CustomerId == request.CustomerId.Value, cancellationToken);

        var productLinks = new ProductSmartLinkSummaryDto();
        if (request.ProductId.HasValue || request.ProductSkuId.HasValue)
        {
            productLinks.SalesLines = await dbContext.SalesOrders.AsNoTracking()
                .Where(x => x.CompanyId == request.CompanyId)
                .SelectMany(x => x.Lines)
                .CountAsync(line =>
                    (!request.ProductId.HasValue || line.ProductId == request.ProductId.Value)
                    && (!request.ProductSkuId.HasValue || line.ProductSkuId == request.ProductSkuId.Value),
                    cancellationToken);
        }

        return new GetSalesOrderSmartLinksResult(partnerLinks, productLinks);
    }
}
