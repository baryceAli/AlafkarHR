namespace Inventory.Warehouses.Features.Inventories.InventoryQueries.GetInventoriesByCompany;

public record GetInventoriesByCompanyResponse(PaginatedResult<InventoryAggregateDto> InventoryList);
public class GetInventoriesByCompanyEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/inventory/inventories/company/{companyId}", async ([FromRoute]Guid companyId,[AsParameters] PaginationRequest request, [FromServices] ISender sender) =>
        {
            var query = new GetInventoriesByCompanyQuery(companyId,request);
            var result = await sender.Send(query);
            return Results.Ok(result.Adapt<GetInventoriesByCompanyResponse>());
        })
            .WithName("GetInventoriesByCompany")
            .Produces<GetInventoriesByCompanyResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Inventories By Company")
            .WithDescription("Get Inventories By Company")
            .RequireAuthorization(PermissionList.InventoryPermissions.View);
    }
}

