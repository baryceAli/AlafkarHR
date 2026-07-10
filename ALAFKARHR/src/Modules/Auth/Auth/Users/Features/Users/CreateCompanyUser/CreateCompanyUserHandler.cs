using Organization.Contracts.Companies.Features.CanAddUserToCompany;
using Shared.Contracts.Organization;

namespace Auth.Users.Features.Users.CreateCompanyUser;

public record CreateCompanyUserCommand(Guid CompanyId, CreateCompanyUserDto User) : ICommand<CreateCompanyUserResult>;
public record CreateCompanyUserResult(Guid UserId, int AssignedRolesCount, int BranchAssignmentsCount);

public class CreateCompanyUserCommandValidator : AbstractValidator<CreateCompanyUserCommand>
{
    public CreateCompanyUserCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.User.UserName).NotEmpty();
        RuleFor(x => x.User.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.User.PhoneNumber).NotEmpty();
        RuleFor(x => x.User.Password).NotEmpty();
        RuleFor(x => x.User.DefaultBranchId)
            .Must((command, defaultBranchId) => defaultBranchId is null || command.User.BranchIds.Contains(defaultBranchId.Value))
            .WithMessage("Default branch must be included in selected branch access.");
    }
}

public class CreateCompanyUserHandler(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    IOptions<OTPOptions> otpOptions,
    IHttpContextAccessor httpContextAccessor,
    ISender sender)
    : ICommandHandler<CreateCompanyUserCommand, CreateCompanyUserResult>
{
    public async Task<CreateCompanyUserResult> Handle(CreateCompanyUserCommand command, CancellationToken cancellationToken)
    {
        var currentCompanyId = GetCurrentCompanyId();
        if (command.CompanyId != currentCompanyId)
            throw new BadRequestException("Cannot create users for another company.");

        var roleNames = command.User.RoleNames
            .Where(roleName => !string.IsNullOrWhiteSpace(roleName))
            .Select(roleName => roleName.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var branchIds = command.User.BranchIds
            .Where(branchId => branchId != Guid.Empty)
            .Distinct()
            .ToList();

        if (command.User.DefaultBranchId.HasValue && !branchIds.Contains(command.User.DefaultBranchId.Value))
            throw new BadRequestException("Default branch must be included in selected branch access.");

        var userLimit = await sender.Send(new CanAddUserToCompanyQuery(command.CompanyId), cancellationToken);
        if (!userLimit.CanAdd)
            throw new BadRequestException(userLimit.Reason ?? "User license limit has been reached");

        var userName = UserNameKeyNormalizer.Normalize(command.User.UserName);
        if (string.IsNullOrWhiteSpace(userName))
            throw new BadRequestException("User name is required.");

        if (await userManager.FindByNameAsync(userName) is not null)
            throw new BadRequestException($"User name already exists: {userName}");

        if (await userManager.FindByEmailAsync(command.User.Email.Trim()) is not null)
            throw new BadRequestException($"Email already exists: {command.User.Email}");

        await CompanyRoleTemplates.SeedDefaultRolesAsync(roleManager, command.CompanyId);
        var roles = await ResolveAssignableRolesAsync(command.CompanyId, roleNames, cancellationToken);

        var user = ApplicationUser.Create(
            Guid.NewGuid(),
            userName,
            command.User.Email.Trim(),
            command.User.PhoneNumber.Trim(),
            UserType.SystemUser,
            GenerateOTP.Generate(otpOptions.Value.Length),
            OTPType.ConfirmEmail,
            DateTime.UtcNow.AddMinutes(otpOptions.Value.ExpirationMinutes),
            command.CompanyId);

        var result = await userManager.CreateAsync(user, command.User.Password);
        if (!result.Succeeded)
            throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));

        try
        {
            if (roles.Count > 0)
            {
                var roleResult = await userManager.AddToRolesAsync(user, roles.Select(role => role.Name!));
                if (!roleResult.Succeeded)
                    throw new BadRequestException(string.Join(", ", roleResult.Errors.Select(e => e.Description)));
            }

            var branchAssignmentsCount = 0;
            if (branchIds.Count > 0)
            {
                var branchResult = await sender.Send(
                    new AssignUserBranchesCommand(user.Id, command.CompanyId, branchIds, command.User.DefaultBranchId),
                    cancellationToken);
                branchAssignmentsCount = branchResult.AssignedCount;
            }

            return new CreateCompanyUserResult(user.Id, roles.Count, branchAssignmentsCount);
        }
        catch
        {
            await userManager.DeleteAsync(user);
            throw;
        }
    }

    private async Task<List<ApplicationRole>> ResolveAssignableRolesAsync(
        Guid companyId,
        List<string> roleNames,
        CancellationToken cancellationToken)
    {
        var roles = new List<ApplicationRole>();
        var companyAdminRoleName = CompanyRoleTemplates.BuildSystemAdminRoleName(companyId);

        foreach (var roleName in roleNames)
        {
            if (string.Equals(roleName, companyAdminRoleName, StringComparison.OrdinalIgnoreCase))
                throw new BadRequestException("Company admin roles cannot be assigned from this create-user flow.");

            var role = await roleManager.FindByNameAsync(roleName)
                ?? throw new NotFoundException($"Role not found: {roleName}");

            if (role.CompanyId != companyId)
                throw new BadRequestException("Cannot assign a role from another company.");

            if (string.Equals(role.Name, companyAdminRoleName, StringComparison.OrdinalIgnoreCase))
                throw new BadRequestException("Company admin roles cannot be assigned from this create-user flow.");

            if (!string.IsNullOrWhiteSpace(role.TemplateKey))
            {
                role = await roleManager.Roles
                    .FirstOrDefaultAsync(r => r.CompanyId == companyId && r.TemplateKey == role.TemplateKey, cancellationToken)
                    ?? throw new NotFoundException($"Role not found: {roleName}");
            }

            roles.Add(role);
        }

        return roles
            .GroupBy(role => role.Name, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .ToList();
    }

    private Guid GetCurrentCompanyId()
    {
        var value = httpContextAccessor.HttpContext?.User?.FindFirst("company_id")?.Value;
        if (!Guid.TryParse(value, out var companyId))
            throw new UnauthorizedAccessException("Current user is not linked to a company.");

        return companyId;
    }
}
