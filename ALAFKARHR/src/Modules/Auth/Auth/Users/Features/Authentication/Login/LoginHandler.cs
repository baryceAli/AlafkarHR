using Auth.Helpers;

namespace Auth.Users.Features.Authentication.Login;

public record LoginCommand(
    LoginDto Login
) : ICommand<LoginResult>;
//public record LoginCommand(
//    //string UserName,
//    string Email,
//    string Password
//) : ICommand<LoginResult>;

//public record LoginResult(
//    string AccessToken,
//    string RefreshToken);
public record LoginResult(LoginResponseDto Login);

public class LoginHandler(
    AuthDbContext dbContext,
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IJwtTokenGenerator tokenGenerator)
    : ICommandHandler<LoginCommand, LoginResult>
{
    public async Task<LoginResult> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(command.Login.Email);

        if (user == null)
            throw new Exception("Invalid credentials");

        var result = await signInManager.CheckPasswordSignInAsync(user, command.Login.Password, false);

        if (!result.Succeeded)
            throw new Exception("Invalid credentials");

        var roles = await userManager.GetRolesAsync(user);

        //var accessToken = tokenGenerator.GenerateToken(user, roles);
        var accessToken = await tokenGenerator.GenerateTokenAsync(user);
        //var currentRefreshToken = Guid.NewGuid().ToString();

        //Add RefreshToken
        var refreshToken = RefreshToken.Create(
            user.Id,
            Guid.NewGuid().ToString(),
            DateTime.UtcNow.AddDays(7),
            user.Email);

        dbContext.Set<RefreshToken>().Add(refreshToken);


        //var refreshToken =Models.RefreshToken.Create(Guid.NewGuid().ToString(),DateTime.UtcNow.AddDays(10),user.Email);
        //user.AddRefreshToken(user.Id,currentRefreshToken, DateTime.UtcNow.AddDays(10), user.Email);
        //dbContext.RefreshTokens.Add(refreshToken);
        //await dbContext.SaveChangesAsync();
        await userManager.UpdateAsync(user);

        //return new LoginResult(accessToken, user.GetActiveRefreshToken(refreshToken.Token).Token);
        return new LoginResult(new LoginResponseDto {AccessToken= accessToken,RefreshToken= user.GetActiveRefreshToken(refreshToken.Token).Token });
    }
}