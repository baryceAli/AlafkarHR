namespace Inventory.Warehouses.Features.Batches.GetBatchesByProductSKU;


public record GetBatchesByProductSKUResponse(PaginatedResult<BatchDto> BatchList);

public class GetBatchesByProductSKUEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/inventory/batches/productSkuId/{productSkuId}", async ([FromRoute] Guid productSkuId, [AsParameters] PaginationRequest request, [FromServices] ISender sender) =>
        {
            var query = new GetBatchesByProductSKUQuery(productSkuId, request);
            var result = await sender.Send(query);
            var response = new GetBatchesByProductSKUResponse(result.BatchList);
            return Results.Ok(response);
        })
            .WithName("GetBatchesByProductSKU")
            .Produces<GetBatchesByProductSKUResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Batches By ProductSKU")
            .WithDescription("Get Batches By ProductSKU")
            .RequireAuthorization(PermissionList.BatchPermissions.View);

    }
}
