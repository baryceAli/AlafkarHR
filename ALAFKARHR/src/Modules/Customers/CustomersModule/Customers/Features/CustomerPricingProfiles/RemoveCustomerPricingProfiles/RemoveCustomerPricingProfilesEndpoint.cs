using Microsoft.AspNetCore.Mvc;

namespace CustomersModule.Customers.Features.CustomerPricingProfiles.RemoveCustomerPricingProfiles;

public record RemoveCustomerPricingProfilesResponse(bool IsSuccess);
public class RemoveCustomerPricingProfilesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/v1/customers/customerPricingProfile/{id}", async ([FromRoute] Guid id, ISender sender) =>
        {
            var result = await sender.Send(new RemoveCustomerPricingProfilesCommand(id));
            return Results.Ok(result.Adapt<RemoveCustomerPricingProfilesResponse>());
        })
            .WithName("RemoveCustomerPricingProfiles")
            .Produces<RemoveCustomerPricingProfilesResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("RemoveCustomerPricingProfiles")
            .WithDescription("RemoveCustomerPricingProfiles")
            .RequireAuthorization(PermissionList.CustomerPricingProfilePermissions.Delete);
    }
}
