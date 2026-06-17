namespace SalesOrder.Orders.Features.CreateOrder;

public record CreateOrderCommand(SalesOrderDto SalesOrder) : ICommand<CreateOrderResult>;
public record CreateOrderResult(Guid Id);
public class CreateOrderHandler(SalesOrderDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
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
            var orderSubtotal = request.SalesOrder.Lines.Sum(line => line.Quantity * line.UnitPrice);
            if (orderSubtotal <= 0m)
                orderSubtotal = request.SalesOrder.Subtotal;

            foreach(var line in request.SalesOrder.Lines)
            {
                var resolvedPrice = await sender.Send(
                    new ResolvePriceQuery(
                        request.SalesOrder.CustomerId,
                        line.ProductSkuId,
                        line.UnitOfMeasureId,
                        line.Quantity,
                        request.SalesOrder.CompanyId,
                        request.SalesOrder.PriceListId,
                        line.TaxRate,
                        order.OrderDate,
                        request.SalesOrder.CouponCode,
                        orderSubtotal),
                    cancellationToken);

                if (!request.SalesOrder.PriceListId.HasValue && resolvedPrice.Price.PriceListId.HasValue)
                    order.ApplyResolvedPriceList(resolvedPrice.Price.PriceListId);

                order.AddLine(line.ProductId,
                    line.ProductSkuId,
                    line.ProductName,
                    line.ProductNameEng,
                    line.SkuCode,
                    line.Quantity,
                    resolvedPrice.Price.UnitPrice,
                    line.UnitOfMeasureId,
                    resolvedPrice.Price.DiscountRate,
                    resolvedPrice.Price.TaxRate,
                    line.Notes,
                    user);
            }
        }

        await dbContext.SalesOrders.AddAsync(order,cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateOrderResult(order.Id);
    }
}
