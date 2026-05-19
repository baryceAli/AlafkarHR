namespace CustomersModule.Customers.Features.CustomerPricingProfiles.UpdateCustomerPricingProfiles;


public record UpdateCustomerPricingProfilesRequest(CustomerPricingProfileDto CustomerPricingProfile);
public record UpdateCustomerPricingProfilesResponse(bool IsSuccess);
public class UpdateCustomerPricingProfilesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/customers/customerPricingProfile", async (UpdateCustomerPricingProfilesRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.Adapt<UpdateCustomerPricingProfilesCommand>());
            return Results.Ok(result.Adapt<UpdateCustomerPricingProfilesResponse>());
        })
            .WithName("UpdateCustomerPricingProfiles")
            .Produces<UpdateCustomerPricingProfilesResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("UpdateCustomerPricingProfiles")
            .WithDescription("UpdateCustomerPricingProfiles")
            .RequireAuthorization(PermissionList.CustomerPricingProfilePermissions.Edit);
    }
}
