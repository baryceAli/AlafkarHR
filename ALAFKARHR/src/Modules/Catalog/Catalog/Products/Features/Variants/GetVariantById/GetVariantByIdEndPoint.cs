using MediatR;

namespace Catalog.Products.Features.Variants.GetVariantById;

public record GetVariantByIdResponse(VariantDto Variant);

public class GetVariantByIdEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/catalog/variants/{id}", async ([FromRoute] Guid id, [FromServices] ISender sender) =>
        {
            var result = await sender.Send(new GetVariantByIdQuery(id));
            var response = result.Adapt<GetVariantByIdResponse>();
            return Results.Ok(response);
        })
            .WithName("GetVariantById")
            .Produces<GetVariantByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .WithSummary("Get Variant By Id")
            .WithSummary("Get Variant By Id");
    }
}
