using EmployeeModule.Employees.Config;

namespace EmployeeModule.Employees.Features.Specializations.CreateSpecialization;


public record CreateSpecializationRequest(SpecializationDto Specialization);
public record CreateSpecializationResponse(SpecializationDto CreatedSpecialization);
public class CreateSpecializationEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost($"{Utils.URL_PATTERN}/{Utils.Specialization_Endpoint}", async (CreateSpecializationRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.Adapt<CreateSpecializationCommand>());
            return Results.Created($"{Utils.URL_PATTERN}/{Utils.Specialization_Endpoint}/{result.CreatedSpecialization.Id}", result.Adapt<CreateSpecializationResponse>());
        })
            .WithName("CreateSpecialization")
            .Produces<CreateSpecializationResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("CreateSpecialization")
            .WithDescription("CreateSpecialization")
            .RequireAuthorization(PermissionList.SpecializationPermissions.Create);
    }
}
