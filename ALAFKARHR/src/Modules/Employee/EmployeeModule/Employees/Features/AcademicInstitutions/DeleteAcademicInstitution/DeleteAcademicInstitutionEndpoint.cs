using EmployeeModule.Employees.Config;

namespace EmployeeModule.Employees.Features.AcademicInstitutions.DeleteAcademicInstitution;

public record DeleteAcademicInstitutionResponse(bool IsSuccess);
public class DeleteAcademicInstitutionEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete($"{Utils.URL_PATTERN}/{Utils.AcademicInstitution_Endpoint}" + "/{id}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new DeleteAcademicInstitutionCommand(id));
            return Results.Ok(result.Adapt<DeleteAcademicInstitutionResponse>());
        })
            .WithName("DeleteAcademicInstitution")
            .Produces<DeleteAcademicInstitutionResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("DeleteAcademicInstitution")
            .WithDescription("DeleteAcademicInstitution")
            .RequireAuthorization(PermissionList.AcademicInistitutionPermissions.Delete);
    }
}
