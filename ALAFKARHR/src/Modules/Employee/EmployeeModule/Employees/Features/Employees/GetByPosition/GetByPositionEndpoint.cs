using EmployeeModule.Employees.Config;
using Microsoft.AspNetCore.Mvc;
using Shared.Pagination;

namespace EmployeeModule.Employees.Features.Employees.GetByPosition;


//public record GetByPositionRequest()
public record GetByPositionResponse(PaginatedResult<EmployeeDto> EmployeeList);
public class GetByPositionEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet($"{Utils.URL_PATTERN}/{Utils.Employee_Endpoint}/Position" + "/{positionId}", async ([FromRoute] Guid positionId, [AsParameters] PaginationRequest request, [FromServices] ISender sender) =>
        {
            var result = await sender.Send(new GetByPositionQuery(positionId, request));
            return Results.Ok(result.Adapt<GetByPositionResponse>());
        })
            .WithName("GetByPosition")
            .Produces<GetByPositionResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("GetByPosition")
            .WithDescription("GetByPosition");
    }
}
