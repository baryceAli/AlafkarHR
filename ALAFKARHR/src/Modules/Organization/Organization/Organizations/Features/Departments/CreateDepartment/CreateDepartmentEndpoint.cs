namespace Organization.Organizations.Features.Departments.CreateDepartment;


public record CreateDepartmentRequest(DepartmentDto Department);
public record CreateDepartmentResponse(DepartmentDto CreatedDepartment);
public class CreateDepartmentEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost($"{Utils.ROUTE_PATTERN}/{Utils.DepartmentEndpoint}", async (CreateDepartmentRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.Adapt<CreateDepartmentCommand>());
            return Results.Created($"{Utils.ROUTE_PATTERN}/{Utils.DepartmentEndpoint}/{result.CreatedDepartment.Id}", result.Adapt<CreateDepartmentResponse>());

        })
            .WithName("CreateDepartment")
            .Produces<CreateDepartmentResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("CreateDepartment")
            .WithDescription("CreateDepartment")
            .RequireAuthorization(PermissionList.AdministrationPermissions.Create);
    }
}
