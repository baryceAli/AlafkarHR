namespace SalesOrder.Orders.Features.DeliverOrder;


public record DeliverOrderRequest(SalesOrderDto SalesOrder);
public record DeliverOrderResponse(bool IsSuccess);
public class DeliverOrderEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/SalesOrders/Order/Deliver", async (DeliverOrderRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.Adapt<DeliverOrderCommand>());
            return Results.Ok(result.Adapt<DeliverOrderResponse>());
        })
            .WithName("DeliverOrder")
            .Produces<DeliverOrderResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("DeliverOrder")
            .WithDescription("DeliverOrder")
            .RequireAuthorization(PermissionList.SalesOrderPermissions.Edit);
    }
}
