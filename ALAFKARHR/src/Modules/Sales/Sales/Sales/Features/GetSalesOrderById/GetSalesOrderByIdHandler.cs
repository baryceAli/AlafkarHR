namespace Sales.Sales.Features.GetSalesOrderById;

public record GetSalesOrderByIdQuery(Guid Id) : IQuery<GetSalesOrderByIdResult>;
public record GetSalesOrderByIdResult(SalesOrderDto SalesOrder);

public class GetSalesOrderByIdHandler(SalesOrderDbContext dbContext)
    : IQueryHandler<GetSalesOrderByIdQuery, GetSalesOrderByIdResult>
{
    public async Task<GetSalesOrderByIdResult> Handle(GetSalesOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await dbContext.SalesOrders
            .Include(x => x.Lines)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Sales order not found: {request.Id}");

        return new GetSalesOrderByIdResult(order.Adapt<SalesOrderDto>());
    }
}
