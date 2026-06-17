namespace Orders.Orders.Features.GetOrderIntakesByCompany;

public record GetOrderIntakesByCompanyQuery(Guid CompanyId, PaginationRequest PaginationRequest)
    : IQuery<GetOrderIntakesByCompanyResult>;
public record GetOrderIntakesByCompanyResult(PaginatedResult<OrderIntakeDto> Orders);

public class GetOrderIntakesByCompanyHandler(OrdersDbContext dbContext)
    : IQueryHandler<GetOrderIntakesByCompanyQuery, GetOrderIntakesByCompanyResult>
{
    public async Task<GetOrderIntakesByCompanyResult> Handle(GetOrderIntakesByCompanyQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.OrderIntakes
            .Include("_lines")
            .AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId);

        if (!string.IsNullOrWhiteSpace(request.PaginationRequest.SearchText))
        {
            var search = request.PaginationRequest.SearchText;
            query = query.Where(x => x.Number.Contains(search) || (x.CustomerName != null && x.CustomerName.Contains(search)));
        }

        var count = await query.LongCountAsync(cancellationToken);
        var data = await query
            .OrderByDescending(x => x.SubmittedAt)
            .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
            .Take(request.PaginationRequest.PageSize)
            .ToListAsync(cancellationToken);

        return new GetOrderIntakesByCompanyResult(
            new PaginatedResult<OrderIntakeDto>(
                request.PaginationRequest.PageIndex,
                request.PaginationRequest.PageSize,
                count,
                data.Select(x => x.ToDto())));
    }
}
