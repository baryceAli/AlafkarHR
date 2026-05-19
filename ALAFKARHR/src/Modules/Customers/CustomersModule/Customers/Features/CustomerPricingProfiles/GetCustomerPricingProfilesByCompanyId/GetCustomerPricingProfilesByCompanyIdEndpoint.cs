using Microsoft.AspNetCore.Mvc;
using Shared.Pagination;

namespace CustomersModule.Customers.Features.CustomerPricingProfiles.GetCustomerPricingProfilesByCompanyId;

public record GetCustomerPricingProfilesByCompanyIdResponse(PaginatedResult<CustomerPricingProfileDto> CustomerPricingProfileList);
public class GetCustomerPricingProfilesByCompanyIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/customers/customerPricingProfile/company/{companyId}",
            async ([FromRoute] Guid companyId, [AsParameters] PaginationRequest request, [FromServices] ISender sender) =>
        {
            var result = await sender.Send(new GetCustomerPricingProfilesByCompanyIdQuery(companyId, request));
            return Results.Ok(result.Adapt<GetCustomerPricingProfilesByCompanyIdResponse>());
        })
            .WithName("GetCustomerPricingProfilesByCompanyId")
            .Produces<GetCustomerPricingProfilesByCompanyIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("GetCustomerPricingProfilesByCompanyId")
            .WithDescription("GetCustomerPricingProfilesByCompanyId")
            .RequireAuthorization(PermissionList.CustomerPricingProfilePermissions.View);
    }
}
