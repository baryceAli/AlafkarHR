namespace Inventory.Warehouses.Features.Batches.CreateBatch;

public record CreateBatchRequest(CreateBatchDto Batch);
public record CreateBatchResponse(Guid Id);
public class CreateBatchEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/inventory/batches", async (CreateBatchRequest request, ISender sender) =>
        {
            var command = request.Adapt<CreateBatchCommand>();
            var result = await sender.Send(command);
            return Results.Created($"/api/v1/inventory/batches/{result.Id}", result.Adapt<CreateBatchResponse>());
        })
            .WithName("CreateBatch")
            .Produces<CreateBatchResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("CreateBatch")
            .WithDescription("CreateBatch");
    }
}
