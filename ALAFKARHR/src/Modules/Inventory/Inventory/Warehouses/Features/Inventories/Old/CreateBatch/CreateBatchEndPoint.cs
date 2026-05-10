namespace Inventory.Warehouses.Features.Inventories.Create;

public record CreateBatchRequest(CreateBatchDto Batch);
public record CreateBatchResponse(bool IsSuccess);
public class CreateBatchEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/inventory/inventories/batches", async (CreateBatchRequest request, ISender sender) =>
        {
            //api/v1/inventory/inventories/batches
            var command = new CreateBatchCommand(request.Batch);
            var result = await sender.Send(command);
            return Results.Ok(new CreateBatchResponse(result.IsSuccess));
        })
            .WithName("AddBatch")
            .Produces<CreateBatchResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("AddBatch to Inventory")
            .WithDescription("Add a batch stock to an existing inventory")
            .RequireAuthorization(PermissionList.InventoryPermissions.Create);
    }
}
