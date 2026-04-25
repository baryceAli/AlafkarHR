using MediatR;

namespace Catalog.Products.Features.Variants.GetVariants;



public record GetVariantsResponse(PaginatedResult<VariantDto> VariantList);

public class GetVariantsEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/catalog/variants", async ([AsParameters] PaginationRequest request, [FromServices] ISender sender) =>
        {
            var command = new GetVariantsQuery(request);
            var result = await sender.Send(command);
            var response = result.Adapt<GetVariantsResult>();
            return Results.Ok(response);
        })
            .WithName("GetVariants")
            .Produces<GetVariantsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Variants")
            .WithDescription("Get Variants");
    }
}
