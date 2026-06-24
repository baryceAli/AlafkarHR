using Shared.Contracts.Organization;

namespace Auth.Users.Features.Dashboard;

public record GetAuthDashboardQuery(Guid CompanyId) : IQuery<GetAuthDashboardResult>;

public record GetAuthDashboardResult(AuthDashboardDto Dashboard);

public class GetAuthDashboardHandler(AuthDbContext dbContext, ISender sender)
    : IQueryHandler<GetAuthDashboardQuery, GetAuthDashboardResult>
{
    public async Task<GetAuthDashboardResult> Handle(GetAuthDashboardQuery request, CancellationToken cancellationToken)
    {
        var branchAccess = await sender.Send(new GetCurrentUserBranchAccessQuery(request.CompanyId), cancellationToken);
        var branchesResult = await sender.Send(new GetCompanyBranchesForAccountingQuery(request.CompanyId), cancellationToken);
        var userBranchAssignmentsResult = await sender.Send(new GetCompanyUserBranchAssignmentsQuery(request.CompanyId), cancellationToken);
        var storeBranchRolesResult = await sender.Send(new GetCompanyBranchRoleAssignmentsForDashboardQuery(request.CompanyId), cancellationToken);

        var users = await dbContext.Users.AsNoTracking()
            .Where(user => user.CompanyId == request.CompanyId)
            .OrderBy(user => user.UserName)
            .ToListAsync(cancellationToken);

        var roles = await dbContext.Roles.AsNoTracking()
            .Where(role => role.CompanyId == request.CompanyId)
            .OrderBy(role => role.DisplayName ?? role.Name)
            .ToListAsync(cancellationToken);

        var roleIds = roles.Select(role => role.Id).ToHashSet();
        var userIds = users.Select(user => user.Id).ToHashSet();
        var roleAssignments = await dbContext.UserRoles.AsNoTracking()
            .Where(userRole => userIds.Contains(userRole.UserId) && roleIds.Contains(userRole.RoleId))
            .ToListAsync(cancellationToken);

        var userBranchAssignments = userBranchAssignmentsResult.Assignments
            .Where(assignment => userIds.Contains(assignment.UserId))
            .ToList();

        var allowedBranchIds = branchAccess.CanViewAllBranches
            ? branchesResult.Branches.Select(branch => branch.BranchId).ToHashSet()
            : branchAccess.BranchIds.ToHashSet();

        var visibleUserIds = branchAccess.CanViewAllBranches
            ? userIds
            : userBranchAssignments
                .Where(assignment => allowedBranchIds.Contains(assignment.BranchId))
                .Select(assignment => assignment.UserId)
                .ToHashSet();

        var visibleUsers = users
            .Where(user => visibleUserIds.Contains(user.Id))
            .ToList();

        var visibleUserIdSet = visibleUsers.Select(user => user.Id).ToHashSet();
        var visibleRoleAssignments = roleAssignments
            .Where(assignment => visibleUserIdSet.Contains(assignment.UserId))
            .ToList();

        var rolesById = roles.ToDictionary(role => role.Id);
        var companyAdminRoleName = $"SystemAdmin-{request.CompanyId:N}";
        var systemAdminRoleIds = roles
            .Where(role => string.Equals(role.Name, companyAdminRoleName, StringComparison.OrdinalIgnoreCase))
            .Select(role => role.Id)
            .ToHashSet();

        var branchUserCounts = userBranchAssignments
            .Where(assignment => visibleUserIdSet.Contains(assignment.UserId) && allowedBranchIds.Contains(assignment.BranchId))
            .GroupBy(assignment => assignment.BranchId)
            .ToDictionary(group => group.Key, group => group.Select(assignment => assignment.UserId).Distinct().Count());

        var visibleStoreBranchRoles = storeBranchRolesResult.Assignments
            .Where(assignment => visibleUserIdSet.Contains(assignment.UserId) && allowedBranchIds.Contains(assignment.BranchId))
            .ToList();

        var storeBranchRoleCounts = visibleStoreBranchRoles
            .GroupBy(assignment => assignment.BranchId)
            .ToDictionary(group => group.Key, group => group.Count());

        var usersById = visibleUsers.ToDictionary(user => user.Id);
        var branchesById = branchesResult.Branches.ToDictionary(branch => branch.BranchId);

        var dashboard = new AuthDashboardDto
        {
            CompanyId = request.CompanyId,
            CanViewAllBranches = branchAccess.CanViewAllBranches,
            VisibleBranchCount = allowedBranchIds.Count,
            VisibleUserCount = visibleUsers.Count,
            TotalRoleCount = roles.Count,
            AssignedRoleLinkCount = visibleRoleAssignments.Count,
            Branches = branchesResult.Branches
                .Where(branch => allowedBranchIds.Contains(branch.BranchId))
                .OrderByDescending(branch => branch.IsMainBranch)
                .ThenBy(branch => branch.NameEng)
                .Select(branch => new AuthDashboardBranchDto
                {
                    BranchId = branch.BranchId,
                    Code = branch.Code,
                    Name = branch.Name,
                    NameEng = branch.NameEng,
                    IsMainBranch = branch.IsMainBranch,
                    UserCount = branchUserCounts.GetValueOrDefault(branch.BranchId),
                    StoreBranchRoleCount = storeBranchRoleCounts.GetValueOrDefault(branch.BranchId)
                })
                .ToList(),
            Roles = roles
                .Select(role => new AuthDashboardRoleDto
                {
                    RoleName = role.Name ?? string.Empty,
                    DisplayName = string.IsNullOrWhiteSpace(role.DisplayName) ? role.Name ?? string.Empty : role.DisplayName,
                    IsSystemAdminRole = string.Equals(role.Name, companyAdminRoleName, StringComparison.OrdinalIgnoreCase),
                    UserCount = visibleRoleAssignments.Count(assignment => assignment.RoleId == role.Id)
                })
                .OrderByDescending(role => role.UserCount)
                .ThenBy(role => role.DisplayName)
                .ToList(),
            StoreBranchRoles = visibleStoreBranchRoles
                .OrderByDescending(assignment => assignment.CreatedAt)
                .Take(8)
                .Select(assignment =>
                {
                    usersById.TryGetValue(assignment.UserId, out var user);
                    branchesById.TryGetValue(assignment.BranchId, out var branch);
                    return new AuthDashboardStoreBranchRoleDto
                    {
                        AssignmentId = assignment.Id,
                        UserId = assignment.UserId,
                        UserName = user?.UserName ?? string.Empty,
                        BranchId = assignment.BranchId,
                        BranchName = branch?.Name ?? string.Empty,
                        BranchNameEng = branch?.NameEng ?? string.Empty,
                        TemplateKey = assignment.TemplateKey
                    };
                })
                .ToList()
        };

        var nonSystemRoleAssignmentsByUser = visibleRoleAssignments
            .Where(assignment => !systemAdminRoleIds.Contains(assignment.RoleId))
            .GroupBy(assignment => assignment.UserId)
            .ToDictionary(group => group.Key, group => group.Count());

        var branchAssignmentCountsByUser = userBranchAssignments
            .Where(assignment => visibleUserIdSet.Contains(assignment.UserId) && allowedBranchIds.Contains(assignment.BranchId))
            .GroupBy(assignment => assignment.UserId)
            .ToDictionary(group => group.Key, group => group.Select(assignment => assignment.BranchId).Distinct().Count());

        var protectedUserIds = visibleUsers
            .Where(user => string.Equals(user.UserName, "admin", StringComparison.OrdinalIgnoreCase)
                || visibleRoleAssignments.Any(assignment => assignment.UserId == user.Id && systemAdminRoleIds.Contains(assignment.RoleId)))
            .Select(user => user.Id)
            .ToHashSet();

        dashboard.UsersWithoutRoles = visibleUsers
            .Where(user => !protectedUserIds.Contains(user.Id))
            .Where(user => !nonSystemRoleAssignmentsByUser.ContainsKey(user.Id))
            .Take(8)
            .Select(user => ToUserDto(user, 0, branchAssignmentCountsByUser.GetValueOrDefault(user.Id)))
            .ToList();

        dashboard.UsersWithoutRolesCount = visibleUsers
            .Where(user => !protectedUserIds.Contains(user.Id))
            .Count(user => !nonSystemRoleAssignmentsByUser.ContainsKey(user.Id));

        dashboard.ProtectedUsers = visibleUsers
            .Where(user => protectedUserIds.Contains(user.Id))
            .Take(8)
            .Select(user => ToUserDto(
                user,
                visibleRoleAssignments.Count(assignment => assignment.UserId == user.Id),
                branchAssignmentCountsByUser.GetValueOrDefault(user.Id)))
            .ToList();

        return new GetAuthDashboardResult(dashboard);
    }

    private static AuthDashboardUserDto ToUserDto(ApplicationUser user, int assignedRoleCount, int assignedBranchCount) => new()
    {
        UserId = user.Id,
        UserName = user.UserName ?? string.Empty,
        AssignedRoleCount = assignedRoleCount,
        AssignedBranchCount = assignedBranchCount
    };
}
