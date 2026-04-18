using EmployeeModule.Employees.Config;
using Microsoft.AspNetCore.Mvc;
using Shared.Pagination;

namespace EmployeeModule.Employees.Features.AcademicInstitutions.GetAcademicInstitutions;


public record GetAcademicInstitutionsResponse(PaginatedResult<AcademicInstitutionDto> AcademicInstitutionList);
public class GetAcademicInstitutionsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet($"{Utils.URL_PATTERN}/{Utils.AcademicInstitution_Endpoint}/company" + "/{companyId}",
            async ([FromRoute] Guid companyId, [AsParameters] PaginationRequest request, [FromServices] ISender sender) =>
        {
            var result = await sender.Send(new GetAcademicInstitutionsQuery(companyId, request));
            return Results.Ok(result.Adapt<GetAcademicInstitutionsResponse>());

        })
            .WithName("GetAcademicInstitutions")
            .Produces<GetAcademicInstitutionsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("GetAcademicInstitutions")
            .WithDescription("GetAcademicInstitutions")
            .RequireAuthorization(PermissionList.AcademicInistitutionPermissions.View);
    }
}
