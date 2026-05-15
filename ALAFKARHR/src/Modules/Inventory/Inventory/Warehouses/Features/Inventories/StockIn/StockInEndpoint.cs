namespace Inventory.Warehouses.Features.Inventories.StockIn;


public record StockInRequest(CreateInventoryAggregateDto InventoryAggregate);
public record StockInResponse(Guid Id);
public class StockInEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/inventory/inventories/StockIn", async (StockInRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.Adapt<StockInCommand>());
            return Results.Created($"/api/v1/inventory/inventories/{result.Id}", result.Adapt<StockInResponse>());
        })
            .WithName("StockIn")
            .Produces<StockInResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Stock In")
            .WithDescription("Stock In")
            .RequireAuthorization(PermissionList.InventoryPermissions.Create, PermissionList.InventoryItemPermissions.Create);
    }
}
