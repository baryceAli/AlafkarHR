namespace Inventory.Warehouses.Features.Warehouses.RemoveWarehouse;

public record RemoveWarehouseResponse(bool IsSuccess);
public class RemoveWarehouseEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/v1/inventory/warehouses/{id:guid}", async ([FromRoute] Guid id, [FromServices] ISender sender) =>
        {
            var command = new RemoveWarehouseCommand(id);
            var result=await sender.Send(command);
            return Results.Ok(new RemoveWarehouseResponse(result.IsSuccess));
        })
            .WithName("RemoveWarehouse")
            .Produces<RemoveWarehouseResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Removes a warehouse by its ID.")
            .WithDescription("Deletes the warehouse identified by the provided ID. Returns a response indicating whether the operation was successful.")
            .RequireAuthorization(PermissionList.WarehousePermissions.Delete);
    }
}
