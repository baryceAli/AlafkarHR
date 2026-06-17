namespace Orders.Orders.Features.GetOrderIntakeById;

public record GetOrderIntakeByIdQuery(Guid Id) : IQuery<GetOrderIntakeByIdResult>;
public record GetOrderIntakeByIdResult(OrderIntakeDto Order);

public class GetOrderIntakeByIdHandler(OrdersDbContext dbContext)
    : IQueryHandler<GetOrderIntakeByIdQuery, GetOrderIntakeByIdResult>
{
    public async Task<GetOrderIntakeByIdResult> Handle(GetOrderIntakeByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await dbContext.OrderIntakes
            .Include("_lines")
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Order intake not found: {request.Id}");

        return new GetOrderIntakeByIdResult(order.ToDto());
    }
}
