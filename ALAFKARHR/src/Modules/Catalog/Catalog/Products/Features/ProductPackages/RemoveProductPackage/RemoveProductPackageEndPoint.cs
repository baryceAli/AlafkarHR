using MediatR;

namespace Catalog.Products.Features.ProductPackages.RemoveProductPackage;

public record RemoveProductPackageResponse(bool IsSuccess);

public class RemoveProductPackageEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/v1/catalog/packages/{id}", async ([FromRoute] Guid id, [FromServices] ISender sender) =>
        {
            var result = await sender.Send(new RemoveProductPackageCommand(id));
            var response = result.Adapt<RemoveProductPackageResponse>();
            return Results.Ok(response);
        })
            .WithName("RemoveProductPackage")
            .Produces<RemoveProductPackageResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .WithSummary("Remove Product Package")
            .WithDescription("Remove Product Package")
            .RequireAuthorization($"{PermissionList.ProductPackagePermissions.Delete}");
    }
}
