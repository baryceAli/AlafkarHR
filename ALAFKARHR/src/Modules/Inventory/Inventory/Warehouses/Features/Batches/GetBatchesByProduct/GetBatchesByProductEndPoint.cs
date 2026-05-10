namespace Inventory.Warehouses.Features.Batches.GetBatchesByProduct;

public record GetBatchesByProductResponse(PaginatedResult<BatchDto> BatchList);

public class GetBatchesByProductEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/inventory/batches/productId/{productId}", async ([FromRoute] Guid productId, [AsParameters] PaginationRequest request, [FromServices] ISender sender) =>
        {
            var query = new GetBatchesByProductQuery(productId, request);
            var result = await sender.Send(query);
            var response = new GetBatchesByProductResponse(result.BatchList);
            return Results.Ok(response);
        })
            .WithName("GetBatchesByProduct")
            .Produces<GetBatchesByProductResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Batches By Product")
            .WithDescription("Get Batches By Product")
            .RequireAuthorization(PermissionList.BatchPermissions.View);

    }
}
