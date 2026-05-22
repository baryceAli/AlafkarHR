namespace SalesOrder.Orders.Features.DeliverOrder;

public record DeliverOrderCommand(SalesOrderDto SalesOrder) : ICommand<DeliverOrderResult>;
public record DeliverOrderResult(bool IsSuccess);
public class DeliverOrderHandler(SalesOrderDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<DeliverOrderCommand, DeliverOrderResult>
{
    public async Task<DeliverOrderResult> Handle(DeliverOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await dbContext.SalesOrders.Include(o => o.Lines).FirstOrDefaultAsync(o => o.Id == request.SalesOrder.Id, cancellationToken);
        if (order is null)
            throw new NotFoundException($"Order not found: {request.SalesOrder.Id}");

        var user = httpContextAccessor.HttpContext?
                       .User?
                       .FindFirst(ClaimTypes.NameIdentifier)?
                       .Value ??
                       throw new UnauthorizedAccessException("User is not authenticated");

        
        if (request.SalesOrder.Lines.Any())
        {
            var linesToDeliver = request.SalesOrder.Lines.ToDictionary(d => d.Id,d=>d.DeliveredQuantity);
            order.Deliver(linesToDeliver);
            
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new DeliverOrderResult(true);
    }
}
