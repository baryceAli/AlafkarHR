using MediatR;

namespace Catalog.Products.Features.Products.GetProductByCompany;

public record GetProductByCompanyResponse(PaginatedResult<ProductDto> ProductList);
public class GetProductByCompanyEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/catalog/products/company/{companyId}",
            async ([FromRoute] Guid companyId, [AsParameters] PaginationRequest request, [FromServices] ISender sender) =>
        {
            var result = await sender.Send(new GetProductByCompanyQuery(companyId, request));
            return Results.Ok(result.Adapt<GetProductByCompanyResponse>());
        })
            .WithName("GetProductByCompany")
            .Produces<GetProductByCompanyResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("GetProductByCompany")
            .WithDescription("GetProductByCompany")
            .RequireAuthorization(PermissionList.ProductPermissions.View);

    }
}
