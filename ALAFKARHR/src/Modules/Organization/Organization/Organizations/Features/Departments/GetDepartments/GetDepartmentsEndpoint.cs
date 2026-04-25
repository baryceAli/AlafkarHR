namespace Organization.Organizations.Features.Departments.GetDepartments;



public record GetDepartmentsResponse(PaginatedResult<DepartmentDto> DepartmentList);
public class GetDepartmentsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet($"{Utils.ROUTE_PATTERN}/{Utils.DepartmentEndpoint}", async ([AsParameters] PaginationRequest request, [FromServices] ISender sender) =>
        {
            var result = await sender.Send(new GetDepartmentsQuery(request));
            return Results.Ok(result.Adapt<GetDepartmentsResponse>());
        })
            .WithName("GetDepartments")
            .Produces<GetDepartmentsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("GetDepartments")
            .WithDescription("GetDepartments")
            .RequireAuthorization(PermissionList.DepartmentPermissions.View);
    }
}
