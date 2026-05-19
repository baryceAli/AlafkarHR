namespace CustomersModule.Customers.Features.Customers.UpdateCustomer;

public record UpdateCustomerRequest(CustomerDto Customer);
public record UpdateCustomerResponse(bool IsSuccess);
public class UpdateCustomerEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/customers/customer", async (UpdateCustomerRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.Adapt<UpdateCustomerCommand>());
            return Results.Ok(result.Adapt<UpdateCustomerResponse>());
        })
            .WithName("UpdateCustomer")
            .Produces<UpdateCustomerResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("UpdateCustomer")
            .WithDescription("UpdateCustomer")
            .RequireAuthorization(PermissionList.CustomerPermissions.Edit);
    }
}
