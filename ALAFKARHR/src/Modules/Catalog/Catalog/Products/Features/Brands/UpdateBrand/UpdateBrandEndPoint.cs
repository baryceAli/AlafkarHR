using MediatR;

namespace Catalog.Products.Features.Brands.UpdateBrand;

public record UpdateBrandRequest(BrandDto Brand);
public record UpdateBrandResponse(bool IsSuccess);
public class UpdateBrandEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/catalog/brands", async (UpdateBrandRequest request, ISender sender) =>
        {
            var command=request.Adapt<UpdateBrandCommand>();

            var result=await sender.Send(command);

            var response=result.Adapt<UpdateBrandResponse>();

            return Results.Ok(response);
        })
            .WithName("UpdateBrand")
            .Produces<UpdateBrandResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Update Brand")
            .WithDescription("Update Brand")
            .RequireAuthorization($"{PermissionList.BrandPermissions.Edit}");
    }
}
