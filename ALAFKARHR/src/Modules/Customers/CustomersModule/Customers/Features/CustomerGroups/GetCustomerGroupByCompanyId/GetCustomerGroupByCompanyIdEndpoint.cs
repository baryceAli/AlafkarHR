using Microsoft.AspNetCore.Mvc;
using Shared.Pagination;

namespace CustomersModule.Customers.Features.CustomerGroups.GetCustomerGroupByCompanyId;


public record GetCustomerGroupByCompanyIdResponse(PaginatedResult<CustomerGroupDto> CustomerGroupList);
public class GetCustomerGroupByCompanyIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/customers/customerGroup/company/{companyId}",
            async ([FromRoute] Guid companyId, [AsParameters] PaginationRequest request, [FromServices] ISender sender) =>
        {
            var result = await sender.Send(new GetCustomerGroupByCompanyIdQuery(companyId, request));
            return Results.Ok(result.Adapt<GetCustomerGroupByCompanyIdResponse>());
        })
            .WithName("GetCustomerGroupByCompanyId")
            .Produces<GetCustomerGroupByCompanyIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("GetCustomerGroupByCompanyId")
            .WithDescription("GetCustomerGroupByCompanyId")
            .RequireAuthorization(PermissionList.CustomerGroupPermissions.View);
    }
}
