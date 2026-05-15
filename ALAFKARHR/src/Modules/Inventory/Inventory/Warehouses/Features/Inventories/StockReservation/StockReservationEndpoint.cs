namespace Inventory.Warehouses.Features.Inventories.StockReservation;


public record StockReservationRequest(CreateInventoryAggregateDto InventoryAggregate);
public record StockReservationResponse(Guid Id);
public class StockReservationEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/inventory/inventories/StockReservation", async (StockReservationRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.Adapt<StockReservationCommand>());
            return Results.Created($"/api/v1/inventory/inventories/{result.Id}", result.Adapt<StockReservationResponse>());
        })
            .WithName("StockReservation")
            .Produces<StockReservationResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Stock Reservation")
            .WithDescription("Stock Reservation")
            .RequireAuthorization(PermissionList.InventoryPermissions.Edit, PermissionList.InventoryItemPermissions.Edit);
    }
}
