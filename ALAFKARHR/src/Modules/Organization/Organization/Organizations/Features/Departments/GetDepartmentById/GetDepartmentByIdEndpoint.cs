namespace Organization.Organizations.Features.Departments.GetDepartmentById;



public record GetDepartmentByIdResponse(DepartmentDto Department);
public class GetDepartmentByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet($"{Utils.ROUTE_PATTERN}/{Utils.DepartmentEndpoint}/" + "{id}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetDepartmentByIdQuery(id));
            return Results.Ok(result.Adapt<GetDepartmentByIdResponse>());
        })
            .WithName("GetDepartmentById")
            .Produces<GetDepartmentByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("GetDepartmentById")
            .WithDescription("GetDepartmentById");
    }
}
