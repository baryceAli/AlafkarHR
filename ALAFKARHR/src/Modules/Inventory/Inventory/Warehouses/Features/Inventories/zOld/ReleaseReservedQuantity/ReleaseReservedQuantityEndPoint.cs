namespace Inventory.Warehouses.Features.Inventories.ReleaseReservedQuantity;

public record ReleaseReservedQuantityRequest(ReserveQuantityDto ReserveQuantity);
public record ReleaseReservedQuantityResponse(bool IsSuccess);
public class ReleaseReservedQuantityEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/inventory/inventories/release-quantity", async (ReleaseReservedQuantityRequest request, ISender sender) =>
        {
            var command = request.Adapt<ReleaseReservedQuantityCommand>();
            var result = await sender.Send(command);
            return Results.Ok(result.Adapt<ReleaseReservedQuantityResult>());
        })
            .WithName("ReleaseReservedQuantity")
            .Produces<ReleaseReservedQuantityResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("ReleaseReservedQuantity")
            .WithDescription("ReleaseReservedQuantity");
    }
}
