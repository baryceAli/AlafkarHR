namespace Sales.Sales.Features.GetSalesOrderById;

public record GetSalesOrderByIdResponse(SalesOrderDto SalesOrder);

public class GetSalesOrderByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/sales/orders/{id}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetSalesOrderByIdQuery(id));
            return Results.Ok(result.Adapt<GetSalesOrderByIdResponse>());
        })
        .WithName("GetSalesOrderById")
        .Produces<GetSalesOrderByIdResponse>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.SalesOrderPermissions.View);
    }
}
