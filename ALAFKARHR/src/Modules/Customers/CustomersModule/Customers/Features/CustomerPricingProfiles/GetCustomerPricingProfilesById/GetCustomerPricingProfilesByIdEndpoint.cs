namespace CustomersModule.Customers.Features.CustomerPricingProfiles.GetCustomerPricingProfilesById;


public record GetCustomerPricingProfilesByIdResponse(CustomerPricingProfileDto CustomerPricingProfile);
public class GetCustomerPricingProfilesByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/customers/customerPricingProfile/{id}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetCustomerPricingProfilesByIdQuery(id));
            return Results.Ok(result.Adapt<GetCustomerPricingProfilesByIdResponse>());
        })
            .WithName("GetCustomerPricingProfilesById")
            .Produces<GetCustomerPricingProfilesByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("GetCustomerPricingProfilesById")
            .WithDescription("GetCustomerPricingProfilesById")
            .RequireAuthorization(PermissionList.CustomerPricingProfilePermissions.View);
    }
}
