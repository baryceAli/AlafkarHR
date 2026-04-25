namespace Inventory.Warehouses.Features.Warehouses.CreateWarehouse;


public record CreateWarehouseRequest(WarehouseDto Warehouse);
public record CreateWarehouseResponse(Guid Id);
public class CreateWarehouseEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/Inventory/warehouses", async (CreateWarehouseRequest request, ISender sender) =>
        {
            var command = new CreateWarehouseCommand(request.Warehouse);
            var result = await sender.Send(command);
            var response = new CreateWarehouseResponse(result.Id);
            return Results.Created($"/api/v1/Inventory/warehouses/{response.Id}", new CreateWarehouseResponse(result.Id));
        })
            .WithName("CreateWarehouse")
            .Produces<CreateWarehouseResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create a new warehouse")
            .WithDescription("Creates a new warehouse with the provided details.")
            .RequireAuthorization(PermissionList.WarehousePermissions.Create);
    }
}
