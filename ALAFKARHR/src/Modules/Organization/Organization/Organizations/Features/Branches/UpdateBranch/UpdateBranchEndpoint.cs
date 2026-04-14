namespace Organization.Organizations.Features.Branches.UpdateBranch;


public record UpdateBranchRequest(BranchDto Branch);
public record UpdateBranchResponse(bool IsSuccess);
public class UpdateBranchEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut($"{Utils.ROUTE_PATTERN}/{Utils.BranchEndpoint}", async (UpdateBranchRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.Adapt<UpdateBranchCommand>());
            return Results.Ok(result.Adapt<UpdateBranchResponse>());
        })
            .WithName("UpdateBranch")
            .Produces<UpdateBranchResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("UpdateBranch")
            .WithDescription("UpdateBranch")
            .RequireAuthorization(PermissionList.BranchPermissions.Edit);
    }
}
