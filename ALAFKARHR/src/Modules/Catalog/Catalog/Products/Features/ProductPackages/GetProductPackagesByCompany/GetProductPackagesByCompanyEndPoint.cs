using MediatR;

namespace Catalog.Products.Features.ProductPackages.GetProductPackagesByCompany;


public record GetProductPackagesByCompanyResponse(Guid CompanyId,PaginatedResult<ProductPackageDto> ProductPackageList);
public class GetProductPackagesByCompanyEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/catalog/packages/company/{companyId}", async ([FromRoute]Guid companyId,[AsParameters] PaginationRequest request, [FromServices] ISender sender) =>
        {
            var query = new GetProductPackagesByCompanyQuery(companyId,request);
            var result = await sender.Send(query);
            var response = result.Adapt<GetProductPackagesByCompanyResponse>();
            return Results.Ok(response);
        })
            .WithName("GetProductPackagesByCompany")
            .Produces<GetProductPackagesByCompanyResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Product Packages By Company")
            .WithDescription("Get Product Packages By Company")
            .RequireAuthorization(PermissionList.ProductPackagePermissions.View);
    }
}
