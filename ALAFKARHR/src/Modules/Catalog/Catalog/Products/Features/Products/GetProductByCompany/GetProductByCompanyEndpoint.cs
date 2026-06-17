using Catalog.Contracts.Products.Features.GetProductByCompany;
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

        app.MapGet("/api/v1/catalog/products/company/{companyId}/priced",
            async (
                [FromRoute] Guid companyId,
                [FromQuery] Guid? customerId,
                [FromQuery] Guid? priceListId,
                [AsParameters] PaginationRequest request,
                [FromServices] ISender sender) =>
        {
            var result = await sender.Send(new GetPricedProductByCompanyQuery(companyId, customerId, priceListId, request));
            return Results.Ok(result.Adapt<GetProductByCompanyResponse>());
        })
            .WithName("GetPricedProductByCompany")
            .Produces<GetProductByCompanyResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get priced products by company")
            .WithDescription("Returns product SKUs with customer-aware prices when a customer is supplied.")
            .RequireAuthorization(PermissionList.ProductPermissions.View);

    }
}
