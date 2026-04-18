using EmployeeModule.Employees.Config;
using Microsoft.AspNetCore.Mvc;
using Shared.Pagination;

namespace EmployeeModule.Employees.Features.Specializations.GetSpecializations;

public record GetSpecializationsResponse(PaginatedResult<SpecializationDto> SpecializationList);
public class GetSpecializationsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet($"{Utils.URL_PATTERN}/{Utils.Specialization_Endpoint}/company" + "/{companyId}",
            async ([FromRoute] Guid companyId, [AsParameters] PaginationRequest request, [FromServices] ISender sender) =>
        {
            var result = await sender.Send(new GetSpecializationsQuery(companyId, request));
            return Results.Ok(result.Adapt<GetSpecializationsResult>());
        })
            .WithName("GetSpecializations")
            .Produces<GetSpecializationsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("GetSpecializations")
            .WithDescription("GetSpecializations")
            .RequireAuthorization(PermissionList.SpecializationPermissions.View);
    }
}
