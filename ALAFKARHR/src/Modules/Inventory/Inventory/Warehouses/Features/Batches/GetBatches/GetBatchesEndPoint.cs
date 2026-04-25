namespace Inventory.Warehouses.Features.Batches.GetBatches;

//public record GetBatchesRequest(PaginationRequest PaginationRequest);
public record GetBatchesResponse(PaginatedResult<BatchDto> BatchList);
public class GetBatchesEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/inventory/batches", async ([AsParameters] PaginationRequest request, [FromServices] ISender sender) =>
        {
            var query = new GetBatchesQuery(request);
            var result = await sender.Send(query);
            var response = new GetBatchesResponse(result.BatchList);
            return Results.Ok(response);
        })
            .WithName("GetBatches")
            .Produces<GetBatchesResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Batches")
            .WithDescription("Get Batches");
    }
}
