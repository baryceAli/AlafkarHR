using MediatR;

namespace Catalog.Products.Features.Brands.GetBrands;

public record GetBrandsResponse(PaginatedResult<BrandDto> Brands);
public class GetBrandsEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/catalog/Brands", async ([AsParameters] PaginationRequest request,ISender sender) =>
        {
            var result=await sender.Send(new GetBrandsQuery(request));
            var response=result.Adapt<GetBrandsResponse>();

            return Results.Ok(response);
        })
            .WithName("GetBrands")
            .Produces<GetBrandsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            //.ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Brands")
            .WithDescription("Get Brands");
    }
}
