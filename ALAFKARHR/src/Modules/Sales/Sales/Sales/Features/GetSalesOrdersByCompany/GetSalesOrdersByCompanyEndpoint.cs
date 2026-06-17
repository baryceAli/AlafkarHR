using Shared.Pagination;

namespace Sales.Sales.Features.GetSalesOrdersByCompany;

public record GetSalesOrdersByCompanyResponse(PaginatedResult<SalesOrderDto> SalesOrders);

public class GetSalesOrdersByCompanyEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/sales/orders/company/{companyId}", async (Guid companyId, [AsParameters] PaginationRequest request, ISender sender) =>
        {
            var result = await sender.Send(new GetSalesOrdersByCompanyQuery(companyId, request));
            return Results.Ok(result.Adapt<GetSalesOrdersByCompanyResponse>());
        })
        .WithName("GetSalesOrdersByCompany")
        .Produces<GetSalesOrdersByCompanyResponse>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.SalesOrderPermissions.View);
    }
}
