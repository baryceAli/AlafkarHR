using EmployeeModule.Employees.Config;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeModule.Employees.Features.Specializations.DeleteSpecialization;

public record DeleteSpecializationResponse(bool IsSuccess);
public class DeleteSpecializationEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete($"{Utils.URL_PATTERN}/{Utils.Specialization_Endpoint}" + "/{id}", async ([FromRoute]Guid id, ISender sender) =>
        {
            var result = await sender.Send(new DeleteSpecializationCommand(id));
            return Results.Ok(result.Adapt<DeleteSpecializationResponse>());
        })
            .WithName("DeleteSpecialization")
            .Produces<DeleteSpecializationResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("DeleteSpecialization")
            .WithDescription("DeleteSpecialization")
            .RequireAuthorization(PermissionList.SpecializationPermissions.Delete);
    }
}
