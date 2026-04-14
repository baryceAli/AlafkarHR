namespace Organization.Organizations.Features.Departments.GetDepartmentsByAdministrationId;


public record GetDepartmentsByAdministrationIdResponse(List<DepartmentDto> DepartmentList);
public class GetDepartmentsByAdministrationIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet($"{Utils.ROUTE_PATTERN}/{Utils.DepartmentEndpoint}" + "getByAdministrationId/{administrationId}",
            async (Guid adminstrationId, ISender sender) =>
        {
            var result = await sender.Send(new GetDepartmentsByAdministrationIdQuery(adminstrationId));
            return Results.Ok(result.Adapt<GetDepartmentsByAdministrationIdResponse>());

        })
            .WithName("GetDepartmentsByAdministrationId")
            .Produces<GetDepartmentsByAdministrationIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("GetDepartmentsByAdministrationId")
            .WithDescription("GetDepartmentsByAdministrationId");
    }
}
