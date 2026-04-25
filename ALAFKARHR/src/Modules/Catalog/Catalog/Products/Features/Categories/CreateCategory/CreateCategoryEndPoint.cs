using MediatR;

namespace Catalog.Products.Features.Categories.CreateCategory;


public record CreateCategoryRequest(CategoryDto Category);
public record CreateCategoryResponse(Guid Id);


public class CreateCategoryEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/catalog/categories", async (CreateCategoryRequest request, ISender sender) =>
        {
            var command= request.Adapt<CreateCategoryCommand>();
            var result=await sender.Send(command);
            var response= result.Adapt<CreateCategoryResponse>();

            return Results.Created($"/api/v1/catalog/categories/{response.Id}", response);
        })
            .WithName("CreateCategory")
            .Produces<CreateCategoryResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create Category")
            .WithDescription("Create Category")
            .RequireAuthorization($"{PermissionList.CategoryPermissions.Create}");

    }
}
