using Shared.Pagination;

namespace Sales.Sales.Features.GetSalesOrdersByCompany;

public record GetSalesOrdersByCompanyResponse(PaginatedResult<SalesOrderDto> SalesOrders);

public class GetSalesOrdersByCompanyEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/sales/orders/company/{companyId}", async (
            Guid companyId,
            [AsParameters] PaginationRequest request,
            Guid? customerId,
            Guid? productId,
            Guid? productSkuId,
            ISender sender) =>
        {
            var result = await sender.Send(new GetSalesOrdersByCompanyQuery(companyId, request, customerId, productId, productSkuId));
            return Results.Ok(result.Adapt<GetSalesOrdersByCompanyResponse>());
        })
        .WithName("GetSalesOrdersByCompany")
        .Produces<GetSalesOrdersByCompanyResponse>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.SalesOrderPermissions.View);

        app.MapGet("/api/v1/sales/orders/smart-links/company/{companyId}", async (
            Guid companyId,
            Guid? customerId,
            Guid? productId,
            Guid? productSkuId,
            ISender sender) =>
        {
            var result = await sender.Send(new GetSalesOrderSmartLinksQuery(companyId, customerId, productId, productSkuId));
            return Results.Ok(new { partnerLinks = result.PartnerLinks, productLinks = result.ProductLinks });
        })
        .WithName("GetSalesOrderSmartLinks")
        .Produces<GetSalesOrderSmartLinksResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.SalesOrderPermissions.View);
    }
}
