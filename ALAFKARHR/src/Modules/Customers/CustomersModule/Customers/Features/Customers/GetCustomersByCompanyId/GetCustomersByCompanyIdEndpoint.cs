using Microsoft.AspNetCore.Mvc;
using Shared.Pagination;

namespace CustomersModule.Customers.Features.Customers.GetCustomersByCompanyId;

public record GetCustomersByCompanyIdResponse(PaginatedResult<CustomerDto> CustomerList);
public class GetCustomersByCompanyIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/customers/customer/company/{companyId}",
            async ([FromRoute] Guid companyId, [AsParameters] PaginationRequest request, [FromServices] ISender sender) =>
        {
            var result = await sender.Send(new GetCustomersByCompanyIdQuery(companyId, request));
            return Results.Ok(result.Adapt<GetCustomersByCompanyIdResponse>());
        })
            .WithName("GetCustomersByCompanyId")
            .Produces<GetCustomersByCompanyIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("GetCustomersByCompanyId")
            .WithDescription("GetCustomersByCompanyId")
            .RequireAuthorization(PermissionList.CustomerPermissions.View);
    }
}
