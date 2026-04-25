using MediatR;

namespace Catalog.Products.Features.Variants.RemoveVariant;


public record RemoveVariantResponse(bool IsSuccess);
public class RemoveVariantEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/v1/catalog/variants/{id}", async ([FromRoute] Guid id, [FromServices] ISender sender) =>
        {
            var result = await sender.Send(new RemoveVariantCommand(id));
            var response = result.Adapt<RemoveVariantResponse>();
            return Results.Ok(response);
        })
            .WithName("RemoveVariant")
            .Produces<RemoveVariantResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .WithSummary("Remove Variant")
            .WithDescription("Remove Variant")
            .RequireAuthorization($"{PermissionList.VariantPermissions.Delete}");
    }
}
