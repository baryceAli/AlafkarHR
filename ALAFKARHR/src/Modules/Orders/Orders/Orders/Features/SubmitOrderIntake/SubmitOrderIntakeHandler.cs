using Orders.Contracts.Orders.Features.SubmitOrderIntake;
using Orders.Orders.Models;

namespace Orders.Orders.Features.SubmitOrderIntake;

public class SubmitOrderIntakeHandler(OrdersDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<SubmitOrderIntakeCommand, SubmitOrderIntakeResult>
{
    public async Task<SubmitOrderIntakeResult> Handle(SubmitOrderIntakeCommand request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";
        var order = OrderIntake.Submit(request.Order, userId);
        await dbContext.OrderIntakes.AddAsync(order, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new SubmitOrderIntakeResult(order.Id, order.Number);
    }
}
