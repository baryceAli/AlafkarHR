namespace CustomersModule.Customers.Features.CustomerGroups.CreateCustomerGroup;


public record CreateCustomerGroupRequest(CustomerGroupDto CustomerGroup);
public record CreateCustomerGroupResponse(Guid Id);
public class CreateCustomerGroupEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/customers/customergroup/", async (CreateCustomerGroupRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.Adapt<CreateCustomerGroupCommand>());
            return Results.Created($"/api/v1/customers/customergroup/{result.Id}", new CreateCustomerGroupResponse(result.Id));
        })
            .WithName("CreateCustomerGroup")
            .Produces<CreateCustomerGroupResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("CreateCustomerGroup")
            .WithDescription("CreateCustomerGroup")
            .RequireAuthorization(PermissionList.CustomerGroupPermissions.Create);
    }
}
