using EmployeeModule.Employees.Config;

namespace EmployeeModule.Employees.Features.AcademicInstitutions.CreateAcademicInstitution;

public record CreateAcademicInstitutionRequest(AcademicInstitutionDto AcademicInstitution);
public record CreateAcademicInstitutionResponse(AcademicInstitutionDto CreatedAcademicInstitution);
public class CreateAcademicInstitutionEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost($"{Utils.URL_PATTERN}/{Utils.AcademicInstitution_Endpoint}", async (CreateAcademicInstitutionRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.Adapt<CreateAcademicInstitutionCommand>());
            
            return Results.Created($"{Utils.URL_PATTERN}/{Utils.AcademicInstitution_Endpoint}/{result.CreatedAcademicInstitute.Id}", new CreateAcademicInstitutionResponse(result.CreatedAcademicInstitute));
        })
            .WithName("CreateAcademicInstitution")
            .Produces<CreateAcademicInstitutionResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("CreateAcademicInstitution")
            .WithDescription("CreateAcademicInstitution")
            .RequireAuthorization(PermissionList.AcademicInistitutionPermissions.Create);
    }
}
