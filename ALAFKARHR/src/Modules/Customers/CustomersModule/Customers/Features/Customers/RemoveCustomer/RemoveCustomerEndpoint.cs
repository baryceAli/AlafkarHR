using Microsoft.AspNetCore.Mvc;

namespace CustomersModule.Customers.Features.Customers.RemoveCustomer;

public record RemoveCustomerResponse(bool IsSuccess);
public class RemoveCustomerEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/v1/customers/customer/id", async ([FromRoute] Guid id, [FromServices] ISender sender) =>
        {
            var result = await sender.Send(new RemoveCustomerCommand(id));
            return Results.Ok(result.Adapt<RemoveCustomerResponse>());
        })
            .WithName("RemoveCustomer")
            .Produces<RemoveCustomerResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("RemoveCustomer")
            .WithDescription("RemoveCustomer")
            .RequireAuthorization(PermissionList.CustomerPermissions.Delete);
    }
}
