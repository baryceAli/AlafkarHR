using MediatR;

namespace Catalog.Products.Features.Brands.CreateBrand;

public record CreateBrandRequest(BrandDto Brand);
public record CreateBrandResponse(Guid Id);

public class CreateBrandEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/catalog/Brands", async (CreateBrandRequest request, ISender sender) =>
        {
            
            var command = request.Adapt<CreateBrandCommand>();
            var result = await sender.Send(command);
            var response = result.Adapt<CreateBrandResponse>();

            return Results.Created($"/api/v1/catalog/Brands/{response.Id}", response);

        })
            .WithName("CreateBrand")
            .Produces<CreateBrandResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create Brand")
            .WithDescription("Create Brand")
            .RequireAuthorization($"{PermissionList.BrandPermissions.Create}");
    }
}
