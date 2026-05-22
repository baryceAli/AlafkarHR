namespace SalesOrder.Orders.Features.CreateOrder;

public record CreateOrderRequest(SalesOrderDto SalesOrder);
public record CreateOrderResponse(Guid Id);
public class CreateOrderEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/SalesOrders/Order", async (CreateOrderRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.Adapt<CreateOrderCommand>());
            return Results.Created($"/api/v1/SalesOrders/Order/{result.Id}", result.Adapt<CreateOrderResponse>());
        })
            .WithName("CreateOrder")
            .Produces<CreateOrderResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("CreateOrder")
            .WithDescription("CreateOrder")
            .RequireAuthorization(PermissionList.SalesOrderPermissions.Create);
    }
}
