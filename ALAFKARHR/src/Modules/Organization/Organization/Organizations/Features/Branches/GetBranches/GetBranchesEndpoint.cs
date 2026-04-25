namespace Organization.Organizations.Features.Branches.GetBranches;


public record GetBranchesResponse(PaginatedResult<BranchDto> BranchList);
public class GetBranchesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet($"{Utils.ROUTE_PATTERN}/{Utils.BranchEndpoint}", async ([AsParameters] PaginationRequest request, [FromServices] ISender sender) =>
        {
            var result = await sender.Send(new GetBranchesQuery(request));
            return Results.Ok(result.Adapt<GetBranchesResponse>());
        })
            .WithName("GetBranches")
            .Produces<GetBranchesResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("GetBranches")
            .WithDescription("GetBranches")
            .RequireAuthorization(PermissionList.BranchPermissions.View);
    }
}
