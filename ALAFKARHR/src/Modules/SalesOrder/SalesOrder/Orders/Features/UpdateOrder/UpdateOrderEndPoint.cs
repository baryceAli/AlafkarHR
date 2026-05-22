namespace SalesOrder.Orders.Features.UpdateOrder;

public record UpdateOrderRequest(SalesOrderDto SalesOrder);
public record UpdateOrderResponse(bool IsSuccess);
public class UpdateOrderEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/SalesOrders/Order", async (UpdateOrderRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.Adapt<UpdateOrderCommand>());
            return Results.Ok(result.Adapt<UpdateOrderResponse>());
        })
            .WithName("UpdateOrder")
            .Produces<UpdateOrderResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("UpdateOrder")
            .WithDescription("UpdateOrder")
            .RequireAuthorization(PermissionList.SalesOrderPermissions.Edit);
    }
}
