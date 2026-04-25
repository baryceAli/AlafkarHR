using MediatR;

namespace Catalog.Products.Features.Categories.UpdateCategory;

public record UpdateCategoryRequest(CategoryDto Category);
public record UpdateCategoryResponse(bool IsSuccess);
public class UpdateCategoryEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/catalog/categories", async (UpdateCategoryRequest request, ISender sender) =>
        {
            var command=request.Adapt<UpdateCategoryCommand>();

            var result=await sender.Send(command);
            var response=result.Adapt<UpdateCategoryResponse>();

            return Results.Ok(response);
        })
            .WithName("UpdateCategory")
            .Produces<UpdateCategoryResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Update Category")
            .WithDescription("Update Category")
            .RequireAuthorization($"{PermissionList.CategoryPermissions.Edit}");
    }
}
