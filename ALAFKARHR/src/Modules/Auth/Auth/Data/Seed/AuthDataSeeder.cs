namespace Auth.Data.Seed;

public class AuthDataSeeder(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IOptions<OTPOptions >oTPOptions)
    : IDataSeeder<AuthDbContext>
{

    public async Task SeedAllAsync(AuthDbContext dbContext)
    {
        

        
        var role = await roleManager.FindByNameAsync("SystemUser");
        if (role is null)
        {
            var result = await roleManager.CreateAsync(new ApplicationRole() { Name = "SystemUser" });
            if (result.Succeeded)
            {
                var addedRole = await roleManager.FindByNameAsync("SystemUser");
                foreach (var permission in PermissionList.GetAll())
                {
                   await roleManager.AddClaimAsync(addedRole, new Claim("Permission", permission));
                }
                //var msg = "Success";
            }
        }
        else
        {
            var roleClaims = await roleManager.GetClaimsAsync(role);
            if(roleClaims is not null && roleClaims.Count < PermissionList.GetAll().Count)
            {
                foreach (var permission in PermissionList.GetAll())
                {
                    if (!roleClaims.Any(rc => rc.Type == "Permission" && rc.Value == permission))
                    {
                        await roleManager.AddClaimAsync(role, new Claim("Permission", permission));
                    }
                }
            }
        }

        role = await roleManager.FindByNameAsync("Customer");
        if (role is null)
        {
            var result = await roleManager.CreateAsync(new ApplicationRole() { Name = "Customer" });
            if (result.Succeeded)
            {
                var msg = "Success";
            }
        }

        role = await roleManager.FindByNameAsync("Driver");
        if (role is null)
        {
            var result = await roleManager.CreateAsync(new ApplicationRole() { Name = "Driver" });
            if (result.Succeeded)
            {

                var msg = "Success";
            }
        }


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
                Guid.Parse("4c3d205f-7e2b-42c2-a081-1700b229d91e"));
            var result = await userManager.CreateAsync(userToRegister, "Admin@123");
            if (result.Succeeded)
            {
                var createdUser = await userManager.FindByNameAsync("admin");


                //await dbContext.SaveChangesAsync();
                var otp = new Random().Next(1000, 9999).ToString();
                createdUser.UpdateOtp(otp,OTPType.ConfirmEmail,DateTime.UtcNow.AddMinutes(5),true);

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
