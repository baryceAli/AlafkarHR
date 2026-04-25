using MediatR;

namespace Catalog.Products.Features.Categories.RemoveCategory;

public record RemoveCategoryResponse(bool IsSuccess);

public class RemoveCategoryEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/v1/catalog/categories/{id}", async ([FromRoute]Guid id, [FromServices]ISender sender) =>
        {
            var result = await sender.Send(new RemoveCategoryCommand(id, "barye@gmail.com"));
            var respose =result.Adapt<RemoveCategoryResponse>();

            return Results.Ok(respose);
        })
            .WithName("RemoveCategory")
            .Produces<RemoveCategoryResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Remove Category")
            .WithDescription("Remove Category")
            .RequireAuthorization($"{PermissionList.CategoryPermissions.Delete}");
    }
}
