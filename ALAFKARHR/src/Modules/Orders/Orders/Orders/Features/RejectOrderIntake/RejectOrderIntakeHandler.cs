namespace Orders.Orders.Features.RejectOrderIntake;

public record RejectOrderIntakeCommand(Guid Id, string Reason) : ICommand<RejectOrderIntakeResult>;
public record RejectOrderIntakeResult(bool IsSuccess);

public class RejectOrderIntakeHandler(OrdersDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<RejectOrderIntakeCommand, RejectOrderIntakeResult>
{
    public async Task<RejectOrderIntakeResult> Handle(RejectOrderIntakeCommand request, CancellationToken cancellationToken)
    {
        var order = await dbContext.OrderIntakes.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Order intake not found: {request.Id}");
        var userId = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";
        order.Reject(request.Reason, userId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new RejectOrderIntakeResult(true);
    }
}
