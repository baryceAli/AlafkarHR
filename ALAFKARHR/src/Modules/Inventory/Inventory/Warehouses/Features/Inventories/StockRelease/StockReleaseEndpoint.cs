namespace Inventory.Warehouses.Features.Inventories.StockRelease;


public record StockReleaseRequest(CreateInventoryAggregateDto InventoryAggregate);
public record StockReleaseResponse(Guid Id);
public class StockReleaseEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/inventory/inventories/StockRelease", async (StockReleaseRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.Adapt<StockReleaseCommand>());
            return Results.Created($"/api/v1/inventory/inventories/{result.Id}", result.Adapt<StockReleaseResponse>());
        })
            .WithName("StockRelease")
            .Produces<StockReleaseResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Stock Release")
            .WithDescription("Stock Release")
            .RequireAuthorization(PermissionList.InventoryPermissions.Edit, PermissionList.InventoryItemPermissions.Edit);
    }
}
