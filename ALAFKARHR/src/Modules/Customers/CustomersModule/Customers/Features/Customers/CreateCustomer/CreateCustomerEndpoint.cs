using CustomersModule.Customers.Models;

namespace CustomersModule.Customers.Features.Customers.CreateCustomer;

public record CreateCustomerRequest(CustomerDto Customer);
public record CreateCustomerResponse(Guid Id);
public class CreateCustomerEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/customers/customer", async (CreateCustomerRequest request, ISender sender) =>
        {
            var result = await sender.Send(new CreateCustomerCommand(request.Customer));
            return Results.Created($"/api/v1/customers/customer/{result.Id}", result.Adapt<CreateCustomerResponse>());
        })
            .WithName("CreateCustomer")
            .Produces<CreateCustomerResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("CreateCustomer")
            .WithDescription("CreateCustomer")
            .RequireAuthorization(PermissionList.CustomerPermissions.Create);
    }
}
