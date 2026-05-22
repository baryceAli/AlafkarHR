namespace SalesOrder.Orders.Features.CompleteOrder;

public record CompleteOrderCommand(SalesOrderDto SalesOrder) : ICommand<CompleteOrderResult>;
public record CompleteOrderResult(bool IsSuccess);
public class CompleteOrderHandler(SalesOrderDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CompleteOrderCommand, CompleteOrderResult>
{
    public async Task<CompleteOrderResult> Handle(CompleteOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await dbContext.SalesOrders.Include(o => o.Lines).FirstOrDefaultAsync(o => o.Id == request.SalesOrder.Id, cancellationToken);
        if (order is null)
            throw new NotFoundException($"Order not found: {request.SalesOrder.Id}");

        var user = httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.NameIdentifier)?
                    .Value ??
                    throw new UnauthorizedAccessException("User is not authenticated");

        order.Complete();
        await dbContext.SaveChangesAsync();
        return new CompleteOrderResult(true);
    }
}
