namespace CustomersModule.Customers.Features.CustomerGroups.UpdateCustomerGroup;


public record UpdateCustomerGroupRequest(CustomerGroupDto CustomerGroup);
public record UpdateCustomerGroupResponse(bool IsSuccess);
public class UpdateCustomerGroupEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/customers/customergroup", async (UpdateCustomerGroupRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.Adapt<UpdateCustomerGroupCommand>());
            return Results.Ok(result.Adapt<UpdateCustomerGroupResponse>());
        })
            .WithName("UpdateCustomerGroup")
            .Produces<UpdateCustomerGroupResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("UpdateCustomerGroup")
            .WithDescription("UpdateCustomerGroup")
            .RequireAuthorization(PermissionList.CustomerGroupPermissions.Edit);
    }
}
