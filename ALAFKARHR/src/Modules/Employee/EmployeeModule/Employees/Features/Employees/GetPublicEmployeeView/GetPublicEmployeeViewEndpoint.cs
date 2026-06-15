namespace EmployeeModule.Employees.Features.Employees.GetPublicEmployeeView;

public record GetPublicEmployeeViewResponse(PublicEmployeeViewDto Employee);

public class GetPublicEmployeeViewEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet($"{Utils.URL_PATTERN}/{Utils.Employee_Endpoint}/public-view/{{id:guid}}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetPublicEmployeeViewQuery(id));
            return Results.Ok(result.Adapt<GetPublicEmployeeViewResponse>());
        })
            .WithName("GetPublicEmployeeView")
            .Produces<GetPublicEmployeeViewResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get public employee view")
            .WithDescription("Returns the limited employee card fields that are intentionally public.")
            .AllowAnonymous();
    }
}
