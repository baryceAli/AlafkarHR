using MediatR;

namespace Catalog.Products.Features.ProductPackages.CreateProductPackage;


public record AddProductPackageRequest(ProductPackageDto ProductPackage);
public record AddProductPackageResponse(Guid Id);
public class CreateProductPackageEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/catalog/packages", async (AddProductPackageRequest request, ISender sender) =>
        {
            var command = request.Adapt<AddProductPackageCommand>();
            var result = await sender.Send(command);

            var response = result.Adapt<AddProductPackageResponse>();

            return Results.Created($"/api/v1/catalog/packages/{response.Id}", response);
        })
            .WithName("CreateProductPackage")
            .Produces<AddProductPackageResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create Product Package")
            .WithDescription("Create Product Package")
            .RequireAuthorization($"{PermissionList.ProductPackagePermissions.Create}");
    }
}
