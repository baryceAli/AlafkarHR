using MediatR;

namespace Catalog.Products.Features.Products.GenerateVariantMatrix;

public record GenerateVariantMatrixRequest(ProductSkuVariantMatrixRequest Request);
public record GenerateVariantMatrixResponse(ProductSkuVariantMatrixResultDto Result);

public class GenerateVariantMatrixEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/catalog/products/skus/generate-variant-matrix", async (
                GenerateVariantMatrixRequest request,
                ISender sender) =>
            {
                var result = await sender.Send(new GenerateVariantMatrixCommand(request.Request));
                return Results.Ok(new GenerateVariantMatrixResponse(result.Result));
            })
            .WithName("GenerateProductSkuVariantMatrix")
            .Produces<GenerateVariantMatrixResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Generate missing SKU variant combinations")
            .RequireAuthorization(PermissionList.ProductPermissions.Create);
    }
}
