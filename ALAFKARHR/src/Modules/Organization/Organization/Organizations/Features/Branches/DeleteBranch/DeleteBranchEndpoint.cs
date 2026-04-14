namespace Organization.Organizations.Features.Branches.DeleteBranch;

public record DeleteBranchResponse(bool IsSuccess);
public class DeleteBranchEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete($"{Utils.ROUTE_PATTERN}/{Utils.BranchEndpoint}/" + "{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new DeleteBranchCommand(id));
            return Results.Ok(result.Adapt<DeleteBranchResponse>());
        })
            .WithName("DeleteBranch")
            .Produces<DeleteBranchResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("DeleteBranch")
            .WithDescription("DeleteBranch")
            .RequireAuthorization(PermissionList.BranchPermissions.Delete);
    }
}
