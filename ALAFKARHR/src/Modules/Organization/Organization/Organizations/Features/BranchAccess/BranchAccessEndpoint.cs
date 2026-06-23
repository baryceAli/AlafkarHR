namespace Organization.Organizations.Features.BranchAccess;

public record AssignUserBranchesRequest(Guid UserId, Guid CompanyId, List<Guid> BranchIds, Guid? DefaultBranchId);

public class BranchAccessEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet($"{Utils.ROUTE_PATTERN}/branch-access/current", async (Guid companyId, ISender sender) =>
        {
            var result = await sender.Send(new GetCurrentUserBranchAccessQuery(companyId));
            return Results.Ok(result);
        })
            .WithName("GetCurrentUserBranchAccess")
            .Produces<GetCurrentUserBranchAccessResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();

        app.MapGet($"{Utils.ROUTE_PATTERN}/branch-access/users/{{userId:guid}}", async (Guid userId, Guid companyId, ISender sender) =>
        {
            var result = await sender.Send(new GetUserBranchAssignmentsQuery(userId, companyId));
            return Results.Ok(result);
        })
            .WithName("GetUserBranchAssignments")
            .Produces<GetUserBranchAssignmentsResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .RequireAuthorization(PermissionList.BranchPermissions.AssignUsers);

        app.MapPut($"{Utils.ROUTE_PATTERN}/branch-access/users/{{userId:guid}}", async (Guid userId, AssignUserBranchesRequest request, ISender sender) =>
        {
            var result = await sender.Send(new AssignUserBranchesCommand(userId, request.CompanyId, request.BranchIds, request.DefaultBranchId));
            return Results.Ok(result);
        })
            .WithName("AssignUserBranches")
            .Produces<AssignUserBranchesResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .RequireAuthorization(PermissionList.BranchPermissions.AssignUsers);
    }
}
