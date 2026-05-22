using Microsoft.EntityFrameworkCore.Update;

namespace SalesOrder.Orders.Features.InvoiceOrder;

public record InvoiceOrderRequest(SalesOrderDto SalesOrder);
public record InvoiceOrderResponse(bool IsSuccess);
public class InvoiceOrderEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/SalesOrder/Order/Invoice", async (InvoiceOrderRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.Adapt<InvoiceOrderCommand>());
            return Results.Ok(result.Adapt<InvoiceOrderResult>());
        })
            .WithName("InvoiceOrder")
            .Produces<InvoiceOrderResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("InvoiceOrder")
            .WithDescription("InvoiceOrder")
            .RequireAuthorization(PermissionList.SalesOrderPermissions.Edit);
    }
}
