using Auth.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Setup;

namespace Auth.Users.Features.SystemSetup;

public sealed class AuthReadinessContributor(AuthDbContext dbContext)
    : ISetupReadinessContributor
{
    public string ModuleKey => "auth";
    public IReadOnlyCollection<SetupStepDefinition> Steps =>
    [
        new("security-access", "core", "Roles & user access", "الأدوار ووصول المستخدمين", "Assign company roles and branch access to every company user.", "عيّن أدوار الشركة والوصول إلى الفروع لكل مستخدم.", "bi-person-lock", "/Auth/Dashboard", PermissionList.UsersPermissions.Select, 40, false)
    ];

    public async Task<IReadOnlyCollection<SetupReadinessResult>> EvaluateAsync(
        Guid companyId,
        CancellationToken cancellationToken)
    {
        var userIds = await dbContext.Users.AsNoTracking()
            .Where(x => x.CompanyId == companyId)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);
        var roleIds = await dbContext.Roles.AsNoTracking()
            .Where(x => x.CompanyId == companyId)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);
        var assignedUserIds = await dbContext.UserRoles.AsNoTracking()
            .Where(x => userIds.Contains(x.UserId) && roleIds.Contains(x.RoleId))
            .Select(x => x.UserId)
            .Distinct()
            .ToListAsync(cancellationToken);

        var complete = userIds.Count > 0 && roleIds.Count > 0 && assignedUserIds.Count == userIds.Count;
        var unassigned = userIds.Count - assignedUserIds.Count;

        return
        [
            new SetupReadinessResult(
                "security-access",
                complete,
                complete ? "Every company user has at least one company role." : $"{unassigned} company user(s) still need a role assignment.",
                complete ? "لكل مستخدم في الشركة دور واحد على الأقل." : $"لا يزال {unassigned} من مستخدمي الشركة بحاجة إلى تعيين دور.")
        ];
    }
}
