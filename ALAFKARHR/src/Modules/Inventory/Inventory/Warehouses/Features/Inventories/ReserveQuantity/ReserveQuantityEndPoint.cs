namespace Inventory.Warehouses.Features.Inventories.ReserveQuantity;

public record ReserveQuantityRequest(ReserveQuantityDto ReserveQuantity);
public record ReserveQuantityResponse(bool IsSuccess);
public class ReserveQuantityEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/inventory/inventories/reserve-quantity", async (ReserveQuantityRequest request, ISender sender) =>
        {
            var command = request.Adapt<ReserveQuantityCommand>();
            var result = await sender.Send(command);
            return Results.Ok(result.Adapt<ReserveQuantityResponse>());
        })
            .WithName("ReserveQuantity")
            .Produces<ReserveQuantityResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("ReserveQuantity")
            .WithDescription("ReserveQuantity");
    }
}
