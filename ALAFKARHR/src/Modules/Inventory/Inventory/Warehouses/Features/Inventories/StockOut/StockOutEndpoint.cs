namespace Inventory.Warehouses.Features.Inventories.StockOut;


public record StockOutRequest(CreateInventoryAggregateDto InventoryAggregate);
public record StockOutResponse(Guid Id);
public class StockOutEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/inventory/inventories/StockOut", async (StockOutRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.Adapt<StockOutCommand>());
            return Results.Created($"/api/v1/inventory/inventories/{result.Id}", result.Adapt<StockOutResponse>());
        })
            .WithName("StockOut")
            .Produces<StockOutResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Stock Out")
            .WithDescription("Stock Out")
            .RequireAuthorization(PermissionList.InventoryPermissions.Edit, PermissionList.InventoryItemPermissions.Edit);
    }
}
