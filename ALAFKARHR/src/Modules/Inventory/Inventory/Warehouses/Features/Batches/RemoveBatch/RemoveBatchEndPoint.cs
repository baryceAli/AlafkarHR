namespace Inventory.Warehouses.Features.Batches.RemoveBatch;

public record RemoveBatchResponse(bool IsSuccess);
public class RemoveBatchEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/v1/inventory/batches/{id:guid}", async (Guid id, ISender sender) =>
        {
            var command = new RemoveBatchCommand(id);
            var result = await sender.Send(command);
            var response = result.Adapt<RemoveBatchResponse>();
            return Results.Ok(response);
        })
            .WithName("RemoveBatch")
            .Produces<RemoveBatchResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Remove Batch")
            .WithDescription("Remove Batch");
    }
}
