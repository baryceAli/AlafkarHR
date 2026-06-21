namespace Auth.Data.Seed;

public class AuthDataSeeder(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    IOptions<OTPOptions> oTPOptions,
    IConfiguration configuration)
    : IDataSeeder<AuthDbContext>
{
    private const string PlatformAdminUserName = "Admin";
    private const string PlatformAdminEmail = "baryce@gmail.com";
    private const string PlatformAdminPhone = "0507804458";
    private const string TenantAdminUserName = "alafkar.admin";
    private const string TenantAdminEmail = "alafkar.admin@alafkarsa.com";
    private const string TenantAdminPhone = "0500000000";

    public async Task SeedAllAsync(AuthDbContext dbContext)
    {
        await CompanyRoleTemplates.EnsurePlatformSystemUserRoleAsync(roleManager);
        await EnsurePlatformCustomerRoleAsync();
        await EnsurePlatformDriverRoleAsync();
        await SyncTenantTemplateRolesAsync(dbContext);
        await EnsurePlatformAdminAsync();
        await EnsureDefaultTenantAdminAsync();
    }

    private async Task SyncTenantTemplateRolesAsync(AuthDbContext dbContext)
    {
        var userCompanyIds = await dbContext.Users
            .AsNoTracking()
            .Where(user => user.CompanyId.HasValue)
            .Select(user => user.CompanyId!.Value)
            .ToListAsync();

        var roleCompanyIds = await roleManager.Roles
            .AsNoTracking()
            .Where(role => role.CompanyId.HasValue)
            .Select(role => role.CompanyId!.Value)
            .ToListAsync();

        var companyIds = userCompanyIds
            .Concat(roleCompanyIds)
            .Append(CompanyRoleTemplates.DefaultCompanyId)
            .Distinct();

        foreach (var companyId in companyIds)
        {
            await CompanyRoleTemplates.SeedDefaultRolesAsync(roleManager, companyId);
        }
    }

    private async Task EnsurePlatformCustomerRoleAsync()
    {
        var role = await roleManager.FindByNameAsync("Customer");
        if (role is not null)
        {
            return;
        }

        await roleManager.CreateAsync(new ApplicationRole
        {
            Name = "Customer",
            DisplayName = "Customer",
            CompanyId = null
        });
    }

    private async Task EnsurePlatformDriverRoleAsync()
    {
        var role = await roleManager.FindByNameAsync("Driver");
        if (role is not null)
        {
            return;
        }

        await roleManager.CreateAsync(new ApplicationRole
        {
            Name = "Driver",
            DisplayName = "Driver",
            CompanyId = null
        });
    }

    private async Task EnsurePlatformAdminAsync()
    {
        var user = await userManager.FindByNameAsync(PlatformAdminUserName);
        if (user is null)
        {
            user = ApplicationUser.Create(
                Guid.NewGuid(),
                PlatformAdminUserName,
                PlatformAdminEmail,
                PlatformAdminPhone,
                UserType.SystemUser,
                GenerateOTP.Generate(oTPOptions.Value.Length),
                OTPType.ConfirmEmail,
                DateTime.UtcNow.AddMinutes(oTPOptions.Value.ExpirationMinutes),
                null);

            var result = await userManager.CreateAsync(user, "Admin@123");
            if (!result.Succeeded)
            {
                throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            user.UpdateOtp(
                GenerateOTP.Generate(oTPOptions.Value.Length),
                OTPType.ConfirmEmail,
                DateTime.UtcNow.AddMinutes(oTPOptions.Value.ExpirationMinutes),
                true);

            await userManager.UpdateAsync(user);
        }

        if (user.CompanyId is not null)
        {
            user.CompanyId = null;
            await userManager.UpdateAsync(user);
        }

        await RemoveTenantRolesFromPlatformAdminAsync(user);

        if (!await userManager.IsInRoleAsync(user, CompanyRoleTemplates.PlatformSystemUserRoleName))
        {
            var result = await userManager.AddToRoleAsync(user, CompanyRoleTemplates.PlatformSystemUserRoleName);
            if (!result.Succeeded)
            {
                throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }

    private async Task RemoveTenantRolesFromPlatformAdminAsync(ApplicationUser user)
    {
        var roleNames = await userManager.GetRolesAsync(user);
        foreach (var roleName in roleNames)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role?.CompanyId is not null)
            {
                var result = await userManager.RemoveFromRoleAsync(user, roleName);
                if (!result.Succeeded)
                {
                    throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }

    private async Task EnsureDefaultTenantAdminAsync()
    {
        var companyId = CompanyRoleTemplates.DefaultCompanyId;
        var role = await EnsureDefaultTenantAdminRoleAsync(companyId);

        await CompanyRoleTemplates.SyncPermissionClaimsAsync(
            roleManager,
            role,
            PermissionList.GetParentCompanyAdminPermissions(),
            removeObsolete: true);

        var tenantAdmin = await userManager.FindByNameAsync(TenantAdminUserName);
        if (tenantAdmin is null)
        {
            var password = configuration["SeedUsers:AlafkarTenantAdmin:TemporaryPassword"];
            if (string.IsNullOrWhiteSpace(password))
            {
                password = "Admin@123";
            }

            tenantAdmin = ApplicationUser.Create(
                Guid.NewGuid(),
                TenantAdminUserName,
                TenantAdminEmail,
                TenantAdminPhone,
                UserType.SystemUser,
                GenerateOTP.Generate(oTPOptions.Value.Length),
                OTPType.ConfirmEmail,
                DateTime.UtcNow.AddMinutes(oTPOptions.Value.ExpirationMinutes),
                companyId);

            var result = await userManager.CreateAsync(tenantAdmin, password);
            if (!result.Succeeded)
            {
                throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            tenantAdmin.UpdateOtp(
                GenerateOTP.Generate(oTPOptions.Value.Length),
                OTPType.ConfirmEmail,
                DateTime.UtcNow.AddMinutes(oTPOptions.Value.ExpirationMinutes),
                true);

            await userManager.UpdateAsync(tenantAdmin);
        }
        else if (tenantAdmin.CompanyId != companyId)
        {
            tenantAdmin.CompanyId = companyId;
            await userManager.UpdateAsync(tenantAdmin);
        }

        if (!await userManager.IsInRoleAsync(tenantAdmin, role.Name!))
        {
            var result = await userManager.AddToRoleAsync(tenantAdmin, role.Name!);
            if (!result.Succeeded)
            {
                throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }

    private async Task<ApplicationRole> EnsureDefaultTenantAdminRoleAsync(Guid companyId)
    {
        var roleName = CompanyRoleTemplates.BuildSystemAdminRoleName(companyId);
        var role = await roleManager.FindByNameAsync(roleName);
        if (role is null)
        {
            role = new ApplicationRole
            {
                Name = roleName,
                DisplayName = "System Admin",
                CompanyId = companyId
            };

            var result = await roleManager.CreateAsync(role);
            if (!result.Succeeded)
            {
                throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            role = await roleManager.FindByNameAsync(roleName)
                ?? throw new Exception($"Couldn't find the role: {roleName}");
        }

        var changed = false;
        if (role.CompanyId != companyId)
        {
            role.CompanyId = companyId;
            changed = true;
        }

        if (string.IsNullOrWhiteSpace(role.DisplayName))
        {
            role.DisplayName = "System Admin";
            changed = true;
        }

        if (changed)
        {
            var result = await roleManager.UpdateAsync(role);
            if (!result.Succeeded)
            {
                throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        return role;
    }
}
