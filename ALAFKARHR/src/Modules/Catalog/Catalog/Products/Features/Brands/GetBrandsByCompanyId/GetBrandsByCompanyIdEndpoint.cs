using MediatR;

namespace Catalog.Products.Features.Brands.GetBrandsByCompanyId;

public record GetBrandsByCompanyIdResponse(PaginatedResult<BrandDto> BrandList);
public class GetBrandsByCompanyIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/catalog/brands/company/{companyId}",
            async ([FromRoute] Guid companyId, [AsParameters] PaginationRequest request, [FromServices] ISender sender) =>
        {
            var result = await sender.Send(new GetBrandsByCompanyIdQuery(companyId, request));
            return Results.Ok(new GetBrandsByCompanyIdResponse(result.BrandList));
        })
            .WithName("GetBrandsByCompanyId")
            .Produces<GetBrandsByCompanyIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("GetBrandsByCompanyId")
            .WithDescription("GetBrandsByCompanyId")
            .RequireAuthorization(PermissionList.BrandPermissions.View);
    }
}
