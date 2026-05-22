namespace SalesOrder.Orders.Features.CreateOrder;

public record CreateOrderCommand(SalesOrderDto SalesOrder) : ICommand<CreateOrderResult>;
public record CreateOrderResult(Guid Id);
public class CreateOrderHandler(SalesOrderDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CreateOrderCommand, CreateOrderResult>
{
    public async Task<CreateOrderResult> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var user = httpContextAccessor.HttpContext?
                       .User?
                       .FindFirst(ClaimTypes.NameIdentifier)?
                       .Value ??
                       throw new UnauthorizedAccessException("User is not authenticated");

        var order=Models.SalesOrder.Create(
            Guid.NewGuid(),
            request.SalesOrder.Number,
            request.SalesOrder.CustomerId,
            request.SalesOrder.PriceListId,
            request.SalesOrder.CompanyId,
            user);

        if (request.SalesOrder.Lines.Any())
        {
            foreach(var line in request.SalesOrder.Lines)
            {
                order.AddLine(line.ProductId,
                    line.ProductSkuId,
                    line.ProductName,
                    line.ProductNameEng,
                    line.SkuCode,
                    line.Quantity,
                    line.UnitPrice,
                    line.UnitOfMeasureId,
                    line.DiscountRate,
                    line.TaxRate,
                    line.Notes,
                    user);
            }
        }

        await dbContext.SalesOrders.AddAsync(order,cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateOrderResult(order.Id);
    }
}
