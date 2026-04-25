namespace Inventory.Warehouses.Features.Inventories.RemoveBatch;

public record RemoveInventoryBatchResponse(bool IsSuccess);
public class RemoveInventoryBatchEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/inventory/inventories/{inventoryId:guid}/batches/{batchId:guid}", async (Guid inventoryId, Guid batchId, ISender sender) =>
        {
            //var command = new RemoveInventoryBatchCommand(inventoryId, batchId);
            //var result = await sender.Send(command);
            //return Results.Ok(new RemoveInventoryBatchResponse(result.IsSuccess));
        })
            .WithName("RemoveInventoryBatch")
            .Produces<RemoveInventoryBatchResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Remove Batch from Inventory")
            .WithDescription("Remove a batch stock from an inventory");
    }
}
