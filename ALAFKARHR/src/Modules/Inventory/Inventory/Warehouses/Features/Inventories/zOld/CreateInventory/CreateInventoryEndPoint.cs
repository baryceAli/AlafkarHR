namespace Inventory.Warehouses.Features.Inventories.CreateInventory;

public record CreateInventoryRequest(CreateInventoryAggregateDto Inventory);
public record CreateInventoryResponse(Guid Id);
public class CreateInventoryEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/inventory/inventories", async (CreateInventoryRequest request, ISender sender) =>
        {
            var command = request.Adapt<CreateInventoryCommand>();
            var result = await sender.Send(command);
            return Results.Created($"/api/inventory/inventories/{result.Id}", result.Adapt<CreateInventoryResult>());
        })
            .WithName("CreateInventory")
            .Produces<CreateInventoryResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("CreateInventory")
            .WithDescription("CreateInventory");
    }
}
