using Auth.Contracts.Features.CreateCompanyAdmin;
using Organization.Contracts.Companies.Features.CanAddUserToCompany;

namespace Auth.Users.Features.CompanyAdmins.CreateCompanyAdmin;

public class CreateCompanyAdminCommandValidator : AbstractValidator<CreateCompanyAdminCommand>
{
    public CreateCompanyAdminCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.CompanyCode).NotEmpty();
        RuleFor(x => x.UserName).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.PhoneNumber).NotEmpty();
        RuleFor(x => x.TemporaryPassword).NotEmpty();
    }
}

public class CreateCompanyAdminHandler(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    IOptions<OTPOptions> oTPOptions,
    ISender sender)
    : ICommandHandler<CreateCompanyAdminCommand, CreateCompanyAdminResult>
{
    public async Task<CreateCompanyAdminResult> Handle(CreateCompanyAdminCommand request, CancellationToken cancellationToken)
    {
        if (userManager.Users.Any(x => x.CompanyId == request.CompanyId))
        {
            var userLimit = await sender.Send(new CanAddUserToCompanyQuery(request.CompanyId), cancellationToken);
            if (!userLimit.CanAdd)
                throw new Exception(userLimit.Reason ?? "User license limit has been reached");
        }

        if (await userManager.FindByNameAsync(request.UserName) is not null)
            throw new Exception($"User name already exists: {request.UserName}");

        if (await userManager.FindByEmailAsync(request.Email) is not null)
            throw new Exception($"Email already exists: {request.Email}");

        var roleName = CompanyRoleTemplates.BuildSystemAdminRoleName(request.CompanyId);
        var role = await roleManager.FindByNameAsync(roleName);
        if (role is null)
        {
            role = new ApplicationRole
            {
                Name = roleName,
                DisplayName = "System Admin",
                CompanyId = request.CompanyId
            };

            var roleResult = await roleManager.CreateAsync(role);
            if (!roleResult.Succeeded)
                throw new Exception(string.Join("Create company admin role, ", roleResult.Errors.Select(e => e.Description)));

            role = await roleManager.FindByNameAsync(roleName)
                ?? throw new Exception($"Couldn't find the role: {roleName}");
        }

        if (string.IsNullOrWhiteSpace(role.DisplayName))
        {
            role.DisplayName = "System Admin";
            await roleManager.UpdateAsync(role);
        }

        var permissions = request.AdminScope == CompanyAdminScope.ParentCompanyAdministration
            ? PermissionList.GetParentCompanyAdminPermissions()
            : PermissionList.GetTenantPermissions();

        await CompanyRoleTemplates.SyncPermissionClaimsAsync(
            roleManager,
            role,
            permissions,
            removeObsolete: true);

        await CompanyRoleTemplates.SeedDefaultRolesAsync(roleManager, request.CompanyId);

        var user = ApplicationUser.Create(
            Guid.NewGuid(),
            request.UserName,
            request.Email,
            request.PhoneNumber,
            UserType.SystemUser,
            GenerateOTP.Generate(oTPOptions.Value.Length),
            OTPType.ConfirmEmail,
            DateTime.UtcNow.AddMinutes(oTPOptions.Value.ExpirationMinutes),
            request.CompanyId);

        var userResult = await userManager.CreateAsync(user, request.TemporaryPassword);
        if (!userResult.Succeeded)
            throw new Exception(string.Join("Create company admin, ", userResult.Errors.Select(e => e.Description)));

        var roleAssignResult = await userManager.AddToRoleAsync(user, roleName);
        if (!roleAssignResult.Succeeded)
            throw new Exception(string.Join("Assign company admin role, ", roleAssignResult.Errors.Select(e => e.Description)));

        return new CreateCompanyAdminResult(user.Id, roleName);
    }
}
