namespace SalesOrder.Orders.Features.CancelOrder;

public record CancelOrderCommand(Guid Id,string Reason) : ICommand<CancelOrderResult>;
public record CancelOrderResult(bool IsSuccess);
public class CancelOrderHandler(SalesOrderDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<CancelOrderCommand, CancelOrderResult>
{
    public async Task<CancelOrderResult> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order=await dbContext.SalesOrders.FirstOrDefaultAsync(o=>o.Id==request.Id,cancellationToken);
        if (order is null)
            throw new NotFoundException($"Order not found: {request.Id}");
        await SalesOrderBranchScope.EnsureCanMutateAsync(sender, order.CompanyId, order.BranchId, cancellationToken);

        var user = httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.NameIdentifier)?
                    .Value ??
                    throw new UnauthorizedAccessException("User is not authenticated");

        order.Cancel(request.Reason,user);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CancelOrderResult(true);
    }
}
