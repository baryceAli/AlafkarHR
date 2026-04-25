namespace Inventory.Warehouses.Features.Inventories.ReserveFIFO;

public record ReserveFIFORequest(Guid InventoryId, decimal Quantity, List<(Guid BatchId, DateTime ExpiryDate)> BatchExpiries);
public record ReserveFIFOResponse(List<ReserveQuantityDto> Allocations);
public class ReserveFIFOEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/inventory/inventories/{inventoryId:guid}/reserve-fifo", async (ReserveFIFORequest request, ISender sender) =>
        {
            var command = request.Adapt<ReserveFIFOCommand>();
            var result = await sender.Send(command);
            return Results.Ok(result.Adapt<ReserveFIFOResponse>());
        })
            .WithName("ReserveFIFO")
            .Produces<ReserveFIFOResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Reserve FIFO")
            .WithDescription("Reserve stock using FIFO across batches based on expiry");
    }
}
