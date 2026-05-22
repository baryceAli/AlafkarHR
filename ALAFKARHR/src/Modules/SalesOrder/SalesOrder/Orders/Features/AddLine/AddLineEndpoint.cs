
namespace SalesOrder.Orders.Features.AddLine;


public record AddLineRequest(SalesOrderLineDto SalesOrderLine);
public record AddLineResponse(Guid Id);
public class AddLineEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/SalesOrders/Order/{id}/AddLIne",
            async ([FromRoute] Guid id, [FromBody] AddLineRequest request, [FromServices] ISender sender) =>
        {
            var result = await sender.Send(new AddLineCommand(id, request.SalesOrderLine));
            return Results.Ok(result.Adapt<AddLineResponse>());
        })
            .WithName("AddLine")
            .Produces<AddLineResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("AddLine")
            .WithDescription("AddLine")
            .RequireAuthorization(PermissionList.SalesOrderPermissions.Create);
    }
}
