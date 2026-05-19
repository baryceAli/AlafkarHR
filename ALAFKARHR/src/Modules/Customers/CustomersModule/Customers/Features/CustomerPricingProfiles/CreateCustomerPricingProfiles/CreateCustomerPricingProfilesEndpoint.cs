namespace CustomersModule.Customers.Features.CustomerPricingProfiles.CreateCustomerPricingProfiles;

public record CreateCustomerPricingProfilesRequest(CustomerPricingProfileDto CustomerPricingProfile);
public record CreateCustomerPricingProfilesResponse(Guid Id);
public class CreateCustomerPricingProfilesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/customers/customerPricingProfile", async (CreateCustomerPricingProfilesRequest request, ISender sender) =>
        {
            var result = await sender.Send(new CreateCustomerPricingProfilesCommand(request.CustomerPricingProfile));
            return Results.Created($"/api/v1/customers/customerPricingProfile/{result.Id}", result.Adapt<CreateCustomerPricingProfilesResponse>());
        })
            .WithName("CreateCustomerPricingProfiles")
            .Produces<CreateCustomerPricingProfilesResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("CreateCustomerPricingProfiles")
            .WithDescription("CreateCustomerPricingProfiles")
            .RequireAuthorization(PermissionList.CustomerPricingProfilePermissions.Create);
    }
}
