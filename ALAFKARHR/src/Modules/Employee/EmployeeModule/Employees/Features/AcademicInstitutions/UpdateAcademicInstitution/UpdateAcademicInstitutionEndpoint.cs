using EmployeeModule.Employees.Config;

namespace EmployeeModule.Employees.Features.AcademicInstitutions.UpdateAcademicInstitution;


public record UpdateAcademicInstitutionRequest(AcademicInstitutionDto AcademicInstitution);
public record UpdateAcademicInstitutionResponse(bool IsSuccess);
public class UpdateAcademicInstitutionEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut($"{Utils.URL_PATTERN}/{Utils.AcademicInstitution_Endpoint}",
            async (UpdateAcademicInstitutionRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.Adapt<UpdateAcademicInstitutionCommand>());
            return Results.Ok(result.Adapt<UpdateAcademicInstitutionResponse>());
        })
            .WithName("UpdateAcademicInstitution")
            .Produces<UpdateAcademicInstitutionResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("UpdateAcademicInstitution")
            .WithDescription("UpdateAcademicInstitution")
            .RequireAuthorization(PermissionList.AcademicInistitutionPermissions.Edit);
    }
}
