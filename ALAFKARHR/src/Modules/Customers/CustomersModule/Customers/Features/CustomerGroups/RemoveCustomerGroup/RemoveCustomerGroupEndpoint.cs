using Microsoft.AspNetCore.Mvc;

namespace CustomersModule.Customers.Features.CustomerGroups.RemoveCustomerGroup;

public record RemoveCustomerGroupResponse(bool IsSuccess);
public class RemoveCustomerGroupEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/v1/customers/customerGroup/{id}", async ([FromRoute] Guid id, [FromServices] ISender sender) =>
        {
            var result = await sender.Send(new RemoveCustomerGroupCommand(id));
            return Results.Ok(result.Adapt<RemoveCustomerGroupResponse>());
        })
            .WithName("RemoveCustomerGroup")
            .Produces<RemoveCustomerGroupResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("RemoveCustomerGroup")
            .WithDescription("RemoveCustomerGroup")
            .RequireAuthorization(PermissionList.CustomerGroupPermissions.Delete);
    }
}
