using EmployeeModule.Employees.Config;

namespace EmployeeModule.Employees.Features.Specializations.UpdateSpecialization;


public record UpdateSpecializationRequest(SpecializationDto specialization);
public record UpdateSpecializationResponse(bool IsSuccess);
public class UpdateSpecializationEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut($"{Utils.URL_PATTERN}/{Utils.Specialization_Endpoint}", async (UpdateSpecializationRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.Adapt<UpdateSpecializationCommand>());
            return Results.Ok(result.Adapt<UpdateSpecializationResponse>());
        })
            .WithName("UpdateSpecialization")
            .Produces<UpdateSpecializationResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("UpdateSpecialization")
            .WithDescription("UpdateSpecialization")
            .RequireAuthorization(PermissionList.SpecializationPermissions.Edit);
    }
}
