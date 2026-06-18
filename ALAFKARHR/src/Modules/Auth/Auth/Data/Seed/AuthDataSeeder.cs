namespace Auth.Data.Seed;

public class AuthDataSeeder(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IOptions<OTPOptions >oTPOptions)
    : IDataSeeder<AuthDbContext>
{

    public async Task SeedAllAsync(AuthDbContext dbContext)
    {
        

        
        var role = await roleManager.FindByNameAsync("SystemUser");
        if (role is null)
        {
            var result = await roleManager.CreateAsync(new ApplicationRole()
            {
                Name = "SystemUser",
                DisplayName = "SystemUser",
                CompanyId = CompanyRoleTemplates.DefaultCompanyId
            });
            if (result.Succeeded)
            {
                var addedRole = await roleManager.FindByNameAsync("SystemUser");
                await CompanyRoleTemplates.SyncPermissionClaimsAsync(
                    roleManager,
                    addedRole!,
                    PermissionList.GetAll(),
                    removeObsolete: false);
                //var msg = "Success";
            }
        }
        else
        {
            if (string.IsNullOrWhiteSpace(role.DisplayName))
            {
                role.DisplayName = role.Name ?? "SystemUser";
                await roleManager.UpdateAsync(role);
            }

            await CompanyRoleTemplates.SyncPermissionClaimsAsync(
                roleManager,
                role,
                PermissionList.GetAll(),
                removeObsolete: false);
        }

        role = await roleManager.FindByNameAsync("Customer");
        if (role is null)
        {
            var result = await roleManager.CreateAsync(new ApplicationRole()
            {
                Name = "Customer",
                DisplayName = "Customer",
                CompanyId = CompanyRoleTemplates.DefaultCompanyId
            });
            if (result.Succeeded)
            {
            }
        }

        role = await roleManager.FindByNameAsync("Driver");
        if (role is null)
        {
            var result = await roleManager.CreateAsync(new ApplicationRole()
            {
                Name = "Driver",
                DisplayName = "Driver",
                CompanyId = CompanyRoleTemplates.DefaultCompanyId
            });
            if (result.Succeeded)
            {
            }
        }

        await CompanyRoleTemplates.SeedDefaultRolesAsync(roleManager, CompanyRoleTemplates.DefaultCompanyId);


        var user = await userManager.FindByNameAsync("admin");
        if (user is null)
        {

            var userToRegister = ApplicationUser.Create(
                Guid.NewGuid(),
                "Admin", 
                "baryce@gmail.com", 
                "0507804458",
                UserType.SystemUser,
                GenerateOTP.Generate(oTPOptions.Value.Length),
                                 OTPType.ConfirmEmail,
                DateTime.UtcNow.AddMinutes(oTPOptions.Value.ExpirationMinutes),
                CompanyRoleTemplates.DefaultCompanyId);
            var result = await userManager.CreateAsync(userToRegister, "Admin@123");
            if (result.Succeeded)
            {
                var createdUser = await userManager.FindByNameAsync("admin");


                //await dbContext.SaveChangesAsync();
                var otp = new Random().Next(1000, 9999).ToString();
                createdUser!.UpdateOtp(otp,OTPType.ConfirmEmail,DateTime.UtcNow.AddMinutes(5),true);

                await userManager.UpdateAsync(createdUser);
                // send Email with OTP to userToRegister.Email
                // probably using a background job to send the email: consider events and a background job processor like Hangfire or Quartz.NET
            }
        }
        user = await userManager.FindByNameAsync("admin");
        if (user != null)
        {
            if (!await userManager.IsInRoleAsync(user, "SystemUser"))
            {
                await userManager.AddToRoleAsync(user, "SystemUser");
            }
        }
    }
}
