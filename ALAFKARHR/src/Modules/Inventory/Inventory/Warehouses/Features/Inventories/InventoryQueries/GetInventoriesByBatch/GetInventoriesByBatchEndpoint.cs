namespace Inventory.Warehouses.Features.Inventories.InventoryQueries.GetInventoriesByBatch;


public record GetInventoriesByBatchResponse(PaginatedResult<InventoryAggregateDto> InventoryList);
public class GetInventoriesByBatchEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/inventory/inventories/batch/{batchId}", 
            async ([FromRoute] Guid batchId, [AsParameters] PaginationRequest request, [FromServices] ISender sender) =>
        {
            var query = new GetInventoriesByBatchQuery(batchId, request);
            var result = await sender.Send(query);
            return Results.Ok(result.Adapt<GetInventoriesByBatchResponse>());
        })
            .WithName("GetInventoriesByBatch")
            .Produces<GetInventoriesByBatchResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("GetInventoriesByBatch")
            .WithDescription("GetInventoriesByBatch")
            .RequireAuthorization(PermissionList.InventoryPermissions.View, PermissionList.InventoryItemPermissions.View,PermissionList.BatchPermissions.View);
    }
}
