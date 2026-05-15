namespace Inventory.Warehouses.Features.Inventories.StockAdjustment;


public record StockAdjustmentRequest(CreateInventoryAggregateDto InventoryAggregate);
public record StockAdjustmentResponse(Guid Id);
public class StockAdjustmentEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/inventory/inventories/StockAdjustment", async (StockAdjustmentRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.Adapt<StockAdjustmentCommand>());
            return Results.Created($"/api/v1/inventory/inventories/{result.Id}", result.Adapt<StockAdjustmentResponse>());
        })
            .WithName("StockAdjustment")
            .Produces<StockAdjustmentResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Stock Adjustment")
            .WithDescription("Stock Adjustment")
            .RequireAuthorization(PermissionList.InventoryPermissions.Create, PermissionList.InventoryItemPermissions.Create);
    }
}
