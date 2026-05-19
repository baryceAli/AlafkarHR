namespace CustomersModule.Customers.Features.CustomerGroups.GetCustomerGroupById;


public record GetCustomerGroupByIdResponse(CustomerGroupDto CustomerGroup);
public class GetCustomerGroupByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/customers/customerGroup/{id}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetCustomerGroupByIdQuery(id));
            return Results.Ok(result.Adapt<GetCustomerGroupByIdResponse>());
        })
            .WithName("GetCustomerGroupById")
            .Produces<GetCustomerGroupByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("GetCustomerGroupById")
            .WithDescription("GetCustomerGroupById")
            .RequireAuthorization(PermissionList.CustomerGroupPermissions.View);
    }
}
