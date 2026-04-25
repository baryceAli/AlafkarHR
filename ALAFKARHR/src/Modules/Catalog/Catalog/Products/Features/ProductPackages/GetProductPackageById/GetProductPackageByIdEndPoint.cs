using MediatR;

namespace Catalog.Products.Features.ProductPackages.GetProductPackageById;

//public record GetProductPackageByIdRequest(Guid Id);
public record GetProductPackageByIdResponse(ProductPackageDto ProductPackage);
public class GetProductPackageByIdEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/catalog/packages/{id:guid}", async ([FromRoute]Guid id,[FromServices] ISender sender) =>
        {
            var result = await sender.Send(new GetProductPackageByIdQuery(id));
            var response = result.Adapt<GetProductPackageByIdResponse>();
            return Results.Ok(response);
        })
            .WithName("GetProductPackagById")
            .Produces<GetProductPackageByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Product Packages By Id")
            .WithDescription("Get Product Packages By Id");


    }
}
