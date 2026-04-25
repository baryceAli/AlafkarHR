namespace Inventory.Warehouses.Features.Inventories.GetInventoryByWarehouseId;


public record GetInventoryByWarehouseIdResponse(List<InventoryAggregateDto> InventoryList);
public class GetInventoryByWarehouseIdEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/inventory/inventories/{warehouseId:guid}", async (Guid warehouseId, ISender sender) =>
        {
            var query = new GetInventoryByWarehouseIdQuery(warehouseId);
            var result = await sender.Send(query);
            return Results.Ok(result.Adapt<GetInventoryByWarehouseIdResponse>());
        })
            .WithName("GetInventoryByWarehouseId")
            .Produces<GetInventoryByWarehouseIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("GetInventoryByWarehouseId")
            .WithDescription("GetInventoryByWarehouseId");

    }
}
