namespace Sales.Sales.Features.GetSalesDashboard;

public record GetSalesDashboardResponse(SalesDashboardDto Dashboard);

public class GetSalesDashboardEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/sales/dashboard/company/{companyId}", async (Guid companyId, ISender sender) =>
        {
            var result = await sender.Send(new GetSalesDashboardQuery(companyId));
            return Results.Ok(result.Adapt<GetSalesDashboardResponse>());
        })
        .WithName("GetSalesDashboard")
        .Produces<GetSalesDashboardResponse>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.SalesOrderPermissions.ViewReports);
    }
}
