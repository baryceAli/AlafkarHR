namespace Organization.Organizations.Features.BranchAccess;

public record AssignUserBranchesRequest(Guid UserId, Guid CompanyId, List<Guid> BranchIds, Guid? DefaultBranchId);
public record AssignUserBranchRoleRequest(Guid UserId, Guid CompanyId, Guid BranchId, string TemplateKey);

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

        app.MapGet($"{Utils.ROUTE_PATTERN}/branch-access/role-profiles", async (ISender sender) =>
        {
            var result = await sender.Send(new GetBranchRoleProfilesQuery());
            return Results.Ok(result);
        })
            .WithName("GetBranchRoleProfiles")
            .Produces<GetBranchRoleProfilesResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.BranchPermissions.AssignUsers);

        app.MapGet($"{Utils.ROUTE_PATTERN}/branch-access/current/roles", async (Guid companyId, ISender sender) =>
        {
            var result = await sender.Send(new GetCurrentUserBranchRoleAccessQuery(companyId));
            return Results.Ok(result.Access);
        })
            .WithName("GetCurrentUserBranchRoleAccess")
            .Produces<CurrentUserBranchRoleAccessDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();

        app.MapGet($"{Utils.ROUTE_PATTERN}/branch-access/users/{{userId:guid}}/roles", async (Guid userId, Guid companyId, ISender sender) =>
        {
            var result = await sender.Send(new GetUserBranchRoleAssignmentsQuery(userId, companyId));
            return Results.Ok(result);
        })
            .WithName("GetUserBranchRoleAssignments")
            .Produces<GetUserBranchRoleAssignmentsResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .RequireAuthorization(PermissionList.BranchPermissions.AssignUsers);

        app.MapGet($"{Utils.ROUTE_PATTERN}/branch-access/roles", async (Guid companyId, Guid? branchId, ISender sender) =>
        {
            var result = await sender.Send(new GetCompanyBranchRoleAssignmentsQuery(companyId, branchId));
            return Results.Ok(result);
        })
            .WithName("GetCompanyBranchRoleAssignments")
            .Produces<GetCompanyBranchRoleAssignmentsResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .RequireAuthorization(PermissionList.BranchPermissions.AssignUsers);

        app.MapPost($"{Utils.ROUTE_PATTERN}/branch-access/users/{{userId:guid}}/roles", async (Guid userId, AssignUserBranchRoleRequest request, ISender sender) =>
        {
            var result = await sender.Send(new AssignUserBranchRoleCommand(userId, request.CompanyId, request.BranchId, request.TemplateKey));
            return Results.Ok(result);
        })
            .WithName("AssignUserBranchRole")
            .Produces<AssignUserBranchRoleResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .RequireAuthorization(PermissionList.BranchPermissions.AssignUsers);

        app.MapDelete($"{Utils.ROUTE_PATTERN}/branch-access/users/roles/{{assignmentId:guid}}", async (Guid assignmentId, ISender sender) =>
        {
            var result = await sender.Send(new RemoveUserBranchRoleCommand(assignmentId));
            return Results.Ok(result);
        })
            .WithName("RemoveUserBranchRole")
            .Produces<RemoveUserBranchRoleResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .RequireAuthorization(PermissionList.BranchPermissions.AssignUsers);
    }
}
