
using MediatR;

namespace Catalog.Products.Features.Brands.RemoveBrand;

public record RemoveBrandResponse(bool IsSuccess);
public class RemoveBrandEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/v1/brands/{id}", async ([FromRoute]Guid id,[FromServices] ISender sender) =>
        {
            var result = await sender.Send(new RemoveBrandCommand(id, "baryce@gmail.com"));
            var response=result.Adapt<RemoveBrandResponse>();

            return Results.Ok(response);
        })

            .WithName("RemoveBrand")
            .Produces<RemoveBrandResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Remove Brand")
            .WithDescription("Remove Brand")
            .RequireAuthorization($"{PermissionList.BrandPermissions.Delete}");
    }
}
