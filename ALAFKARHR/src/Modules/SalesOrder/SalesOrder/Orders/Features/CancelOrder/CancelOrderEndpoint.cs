namespace SalesOrder.Orders.Features.CancelOrder;

public record CancelOrderRequest(Guid Id, string Reason);
public record CancelOrderResponse(bool IsSuccess);
public class CancelOrderEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/SalesOrders/Order/Cancel", async (CancelOrderRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.Adapt<CancelOrderCommand>());
            return Results.Ok(result.Adapt<CancelOrderResponse>());
        })
            .WithName("CancelOrder")
            .Produces<CancelOrderResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("CancelOrder")
            .WithDescription("CancelOrder")
            .RequireAuthorization(PermissionList.SalesOrderPermissions.Edit);
    }
}
