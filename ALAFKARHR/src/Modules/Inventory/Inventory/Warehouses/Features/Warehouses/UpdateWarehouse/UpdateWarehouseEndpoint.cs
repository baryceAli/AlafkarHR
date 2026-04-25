

namespace Inventory.Warehouses.Features.Warehouses.UpdateWarehouse;


public record UpdateWarehouseRequest(WarehouseDto Warehouse);
public record UpdateWarehouseResponse(bool IsSuccess);
public class UpdateWarehouseEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/inventory/warehouses", async (UpdateWarehouseRequest request, ISender sender) =>
        {
            var command=request.Adapt<UpdateWarehouseCommand>();
            var result = await sender.Send(command);
            var response = result.Adapt<UpdateWarehouseResponse>();
            return Results.Ok(response);
        })
            .WithName("UpdateWarehouse")
            .Produces<UpdateWarehouseResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Updates an existing warehouse.")
            .WithDescription("Updates an existing warehouse in the inventory system. The request must include the warehouse's unique identifier and the updated details. Returns a response indicating whether the update was successful.")
            .RequireAuthorization(PermissionList.WarehousePermissions.Edit);
    }
}
