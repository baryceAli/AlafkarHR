using Microsoft.AspNetCore.Mvc;
using Shared.Pagination;

namespace EmployeeModule.Employees.Features.Positions.GetPositions;

public record GetPositionsResponse(PaginatedResult<PositionDto> PositionList);
public class GetPositionsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet($"{Utils.URL_PATTERN}/{Utils.Position_Endpoint}/company" + "/{companyId}",
                            async ([FromRoute] Guid companyId, [AsParameters] PaginationRequest request, [FromServices] ISender sender) =>
        {
            var result = await sender.Send(new GetPositionsQuery(companyId, request));
            return Results.Ok(result.Adapt<GetPositionsResponse>());
        })
            .WithName("GetPositions")
            .Produces<GetPositionsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("GetPositions")
            .WithDescription("GetPositions");
    }
}
