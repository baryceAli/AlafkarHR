namespace Inventory.Warehouses.Features.Inventories.InventoryQueries.GetInventories;

public record GetInventoriesResponse(PaginatedResult<InventoryAggregateDto> InventoryList);
public class GetInventoriesEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/inventory/inventories", async (Guid companyId, Guid? branchId, [AsParameters] PaginationRequest request, [FromServices] ISender sender) =>
        {
            var query = new GetInventoriesQuery(companyId, request, branchId);
            var result = await sender.Send(query);
            return Results.Ok(result.Adapt<GetInventoriesResponse>());
        })
            .WithName("GetInventories")
            .Produces<GetInventoriesResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("GetInventories")
            .WithDescription("GetInventories")
            .RequireAuthorization(PermissionList.InventoryPermissions.View);
    }
}

