namespace Organization.Organizations.Features.Departments.DeleteDepartment;


public record DeleteDepartmentResponse(bool IsSuccess);
public class DeleteDepartmentEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete($"{Utils.ROUTE_PATTERN}/{Utils.DepartmentEndpoint}" + "/{id}", async ([FromRoute] Guid id, [FromServices] ISender sender) =>
        {
            var result = await sender.Send(new DeleteDepartmentCommand(id));
            return Results.Ok(result.Adapt<DeleteDepartmentResponse>());
        })
            .WithName("DeleteDepartment")
            .Produces<DeleteDepartmentResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("DeleteDepartment")
            .WithDescription("DeleteDepartment")
            .RequireAuthorization(PermissionList.DepartmentPermissions.Delete);
    }
}
