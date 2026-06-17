using Orders.Contracts.Orders.Features.AcceptOrderIntake;
using Sales.Contracts.Sales.Features.CreateSalesOrderFromIntake;

namespace Orders.Orders.Features.AcceptOrderIntake;

public class AcceptOrderIntakeHandler(OrdersDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<AcceptOrderIntakeCommand, AcceptOrderIntakeResult>
{
    public async Task<AcceptOrderIntakeResult> Handle(AcceptOrderIntakeCommand request, CancellationToken cancellationToken)
    {
        var order = await dbContext.OrderIntakes
            .Include("_lines")
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Order intake not found: {request.Id}");

        var salesResult = await sender.Send(new CreateSalesOrderFromIntakeCommand(order.ToDto()), cancellationToken);
        var userId = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";
        order.MarkAccepted(salesResult.SalesOrderId, userId);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new AcceptOrderIntakeResult(order.Id, salesResult.SalesOrderId);
    }
}
