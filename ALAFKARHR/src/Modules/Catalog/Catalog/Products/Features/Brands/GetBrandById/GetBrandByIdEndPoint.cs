using MediatR;

namespace Catalog.Products.Features.Brands.GetBrandById;

public record GetBrandByIdResponse(BrandDto Brand);
public class GetBrandByIdEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/catalog/brands/{id}", async (Guid id, ISender sender) =>
        {
            var result=await sender.Send(new GetBrandByIdQuery(id));

            var response=result.Adapt<GetBrandByIdResponse>();

            return Results.Ok(response);
        })
            .WithName("Get Brand")
            .Produces<GetBrandByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Brand")
            .WithDescription("Get Brand");
    }
}
