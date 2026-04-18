using EmployeeModule.Employees.Config;

namespace EmployeeModule.Employees.Features.AcademicInstitutions.GetAcademicInstitutionById;

public record GetAcademicInstitutionByIdResponse(AcademicInstitutionDto AcademicInstitution);
public class GetAcademicInstitutionByIdEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet($"{Utils.URL_PATTERN}/{Utils.AcademicInstitution_Endpoint}" + "/{id}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetAcademicInstitutionByIdQuery(id));
            return Results.Ok(result.Adapt<GetAcademicInstitutionByIdResponse>());
        })
            .WithName("GetAcademicInstitutionById")
            .Produces<GetAcademicInstitutionByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("GetAcademicInstitutionById")
            .WithDescription("GetAcademicInstitutionById")
            .RequireAuthorization(PermissionList.AcademicInistitutionPermissions.View);
    }
}
