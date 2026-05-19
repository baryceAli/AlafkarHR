namespace CustomersModule.Customers.Features.Customers.GetCustomerById;

public record GetCustomerByIdResponse(CustomerDto Customer);
public class GetCustomerByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/customers/customer/{id}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetCustomerByIdQuery(id));
            return Results.Ok(result.Adapt<GetCustomerByIdResponse>());
        })
            .WithName("GetCustomerById")
            .Produces<GetCustomerByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("GetCustomerById")
            .WithDescription("GetCustomerById")
            .RequireAuthorization(PermissionList.CustomerPermissions.View);
    }
}
