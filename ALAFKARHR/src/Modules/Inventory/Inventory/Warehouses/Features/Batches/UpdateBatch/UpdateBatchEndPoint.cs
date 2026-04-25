
namespace Inventory.Warehouses.Features.Batches.UpdateBatch;


public record UpdateBatchRequest(UpdateBatchDto Batch);
public record UpdateBatchResponse(bool IsSuccess);
public class UpdateBatchEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/inventory/batches", async (UpdateBatchRequest request, ISender sender) =>
        {
            var command = request.Adapt<UpdateBatchCommand>();
            var result = await sender.Send(command);
            return Results.Ok(result.Adapt<UpdateBatchResponse>());
        })
            .WithName("UpdateBatch")
            .Produces<UpdateBatchResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("UpdateBatch")
            .WithDescription("UpdateBatch")
            .RequireAuthorization(PermissionList.InventoryPermissions.Edit);
    }
}
