namespace SalesOrder.Orders.Features.ConfirmOrder;

public record ConfirmOrderCommand(Guid Id) : ICommand<ConfirmOrderResult>;
public record ConfirmOrderResult(bool IsSuccess);
public record ConfirmOrderResponse(bool IsSuccess);

public class ConfirmOrderHandler(SalesOrderDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<ConfirmOrderCommand, ConfirmOrderResult>
{
    public async Task<ConfirmOrderResult> Handle(ConfirmOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await dbContext.SalesOrders.Include(x => x.Lines).FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Sales order not found: {request.Id}");

        var eligibility = await sender.Send(
            new GetCustomerSalesEligibilityQuery(order.CustomerId, order.CompanyId, order.TotalAmount),
            cancellationToken);

        if (!eligibility.Exists)
            throw new BadRequestException("Customer was not found for this sales order.");

        if (!eligibility.IsActive)
            throw new BadRequestException(eligibility.BlockReason ?? "Customer is not active.");

        if (!eligibility.IsCreditAllowed)
            throw new BadRequestException(eligibility.BlockReason ?? "Customer credit is not sufficient or is on hold.");

        var user = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                   ?? throw new UnauthorizedAccessException("User is not authenticated");

        try
        {
            order.Confirm();
        }
        catch (Exception ex)
        {
            throw new BadRequestException(ex.Message);
        }
        order.ConfirmedBy = user;
        await dbContext.SaveChangesAsync(cancellationToken);
        return new ConfirmOrderResult(true);
    }
}

public class ConfirmOrderEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/SalesOrders/Order/{id:guid}/Confirm", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new ConfirmOrderCommand(id));
            return Results.Ok(result.Adapt<ConfirmOrderResponse>());
        })
            .WithName("ConfirmOrder")
            .Produces<ConfirmOrderResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Confirm sales order")
            .WithDescription("Confirms a draft sales order.")
            .RequireAuthorization(PermissionList.SalesOrderPermissions.Confirm);
    }
}
