using MediatR;

namespace Catalog.Products.Features.ProductPackages.UpdateProductPackage;


public record UpdateProductPackageRequest(ProductPackageDto ProductPackage);
public record UpdateProductPackageResponse(bool IsSuccess);
public class UpdateProductPackageEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/catalog/packages", async (UpdateProductPackageRequest request, ISender sender) =>
        {
            var command = request.Adapt<UpdateProductPackageCommand>();
            var result = await sender.Send(command);
            var response = result.Adapt<UpdateProductPackageResponse>();
            return Results.Ok(response);
        })
            .WithName("UpdatePackage")
            .Produces<UpdateProductPackageResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Update Package")
            .WithDescription("Update Package")
            .RequireAuthorization($"{PermissionList.ProductPackagePermissions.Edit}");
    }
}
