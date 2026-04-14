namespace Organization.Organizations.Features.Departments.UpdateDepartment;



public record UpdateDepartmentRequest(DepartmentDto Department);
public record UpdateDepartmentResponse(bool IsSuccess);
public class UpdateDepartmentEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut($"{Utils.ROUTE_PATTERN}/{Utils.DepartmentEndpoint}", async (UpdateDepartmentRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.Adapt<UpdateDepartmentCommand>());
            return Results.Ok(result.Adapt<UpdateDepartmentResponse>());
        })
            .WithName("UpdateDepartment")
            .Produces<UpdateDepartmentResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("UpdateDepartment")
            .WithDescription("UpdateDepartment")
            .RequireAuthorization(PermissionList.DepartmentPermissions.Edit);
    }
}
