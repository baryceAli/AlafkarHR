using EmployeeModule.Employees.Config;

namespace EmployeeModule.Employees.Features.Specializations.GetSpecializationById;

public record GetSpecializationByIdResponse(SpecializationDto Specialization);
public class GetSpecializationByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet($"{Utils.URL_PATTERN}/{Utils.Specialization_Endpoint}" + "/{id}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetSpecializationByIdQuery(id));
            return Results.Ok(result.Adapt<GetSpecializationByIdResponse>());
        })
            .WithName("GetSpecializationById")
            .Produces<GetSpecializationByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("GetSpecializationById")
            .WithDescription("GetSpecializationById")
            .RequireAuthorization(PermissionList.SpecializationPermissions.View);
        
    }
}
