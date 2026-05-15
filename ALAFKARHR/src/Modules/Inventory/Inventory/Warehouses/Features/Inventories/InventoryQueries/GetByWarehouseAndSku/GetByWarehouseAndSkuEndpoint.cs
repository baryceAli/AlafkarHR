namespace Inventory.Warehouses.Features.Inventories.InventoryQueries.GetByWarehouseAndSku;

public record GetByWarehouseAndSkuResponse(InventoryAggregateDto InventoryAggregate);
public class GetByWarehouseAndSkuEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/Inventory/inventories/warehouse/{warehouseId}/sku/{skuId}",
            async ([FromRoute] Guid warehouseId, [FromRoute] Guid skuId, [FromServices] ISender sender) =>
        {
            var result = await sender.Send(new GetByWarehouseAndSkuQuery(warehouseId, skuId));
            return Results.Ok(result.Adapt<GetByWarehouseAndSkuResponse>());
        })
            .WithName("GetByWarehouseAndSku")
            .Produces<GetByWarehouseAndSkuResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get By Warehouse And Sku")
            .WithDescription("Get By Warehouse And Sku")
            .RequireAuthorization(PermissionList.InventoryPermissions.View, PermissionList.InventoryItemPermissions.View);
    }
}
