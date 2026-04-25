namespace Inventory.Warehouses.Features.Batches.GetBatchById;

public record GetBatchByIdResponse(BatchDto Batch);
public class GetBatchByIdEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/inventory/batches/{id:guid}", async (Guid id, [FromServices] ISender sender) =>
        {
            var query = new GetBatchByIdQuery(id);
            var result = await sender.Send(query);
            var response = result.Adapt<GetBatchByIdResponse>();
            return Results.Ok(response);
        })
            .WithName("GetBatchById")
            .Produces<GetBatchByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Batch By Id")
            .WithDescription("Get Batch By Id");
    }
}
