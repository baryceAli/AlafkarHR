using Catalog.Products.Features.Products.CreateProduct;
using MediatR;

namespace Catalog.Products.Features.Products.GetProductById;


//public record GetProductByIdRequest(Guid Id);
public record GetProductByIdResponse(ProductDto Product);
public class GetProductByIdEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/catalog/products/{id}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetProductByIdQuery(id));
            var response=result.Adapt<GetProductByIdResponse>();

            return Results.Ok(response);
        })
            .WithName("Get Product")
            .Produces<CreateProductResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Product")
            .WithDescription("Get Product")
            .RequireAuthorization(PermissionList.ProductPermissions.Select); 
    }
}
