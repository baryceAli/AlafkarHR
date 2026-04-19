using EmployeeModule.Employees.Config;
using Microsoft.AspNetCore.Mvc;
using Shared.Pagination;

namespace EmployeeModule.Employees.Features.Specializations.GetSpecializationsByCompany;

public record GetSpecializationsByCompanyResponse(PaginatedResult<SpecializationDto> SpecializationList);
public class GetSpecializationsByCompanyEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet($"{Utils.URL_PATTERN}/{Utils.Specialization_Endpoint}/company" + "/{companyId}",
            async ([FromRoute] Guid companyId, [AsParameters] PaginationRequest request, [FromServices] ISender sender) =>
        {
            var result = await sender.Send(new GetSpecializationsByCompanyQuery(companyId, request));
            return Results.Ok(result.Adapt<GetSpecializationsByCompanyResult>());
        })
            .WithName("GetSpecializationsByCompany")
            .Produces<GetSpecializationsByCompanyResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("GetSpecializationsByCompany")
            .WithDescription("GetSpecializationsByCompany")
            .RequireAuthorization(PermissionList.SpecializationPermissions.View);
    }
}
