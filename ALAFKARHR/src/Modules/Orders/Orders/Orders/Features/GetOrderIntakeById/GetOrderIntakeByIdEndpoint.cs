namespace Orders.Orders.Features.GetOrderIntakeById;

public record GetOrderIntakeByIdResponse(OrderIntakeDto Order);

public class GetOrderIntakeByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/orders/intakes/{id}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetOrderIntakeByIdQuery(id));
            return Results.Ok(result.Adapt<GetOrderIntakeByIdResponse>());
        })
        .WithName("GetOrderIntakeById")
        .Produces<GetOrderIntakeByIdResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization(PermissionList.OrderIntakePermissions.View);
    }
}
