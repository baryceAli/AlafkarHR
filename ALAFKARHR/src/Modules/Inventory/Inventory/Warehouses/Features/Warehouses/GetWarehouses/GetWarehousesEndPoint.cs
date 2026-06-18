namespace Inventory.Warehouses.Features.Warehouses.GetWarehouses
{
    public record GetWarehousesResponse(PaginatedResult<WarehouseDto> WarehouseList);

    public class GetWarehousesEndPoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/v1/inventory/warehouses/company/{companyId}", async ([FromRoute]Guid companyId, WarehouseType? warehouseType, [AsParameters] PaginationRequest paginationRequest, ISender sender) =>
            {
                var result = await sender.Send(new GetWarehousesQuery(companyId, paginationRequest, warehouseType));
                var response = new GetWarehousesResponse(result.WarehouseList);
                return Results.Ok(response);
            })
                .WithName("GetWarehouses")
                .Produces<GetWarehousesResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .WithSummary("Get Warehouses")
                .WithDescription("Get Warehouses")
                .RequireAuthorization(PermissionList.WarehousePermissions.View);
        }
    }
}
