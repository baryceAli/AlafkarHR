using Auth.Helpers;
using Microsoft.Extensions.Logging;

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
    ILogger<LoginHandler> logger,
    IJwtTokenGenerator tokenGenerator)
    : ICommandHandler<LoginCommand, LoginResult>
{
    public async Task<LoginResult> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Login attempt for {Email}", command.Login.Email);

            var user = await userManager.FindByEmailAsync(command.Login.Email);

            if (user == null)
            {
                logger.LogInformation("User not found by email. Trying username.");

                user = await userManager.FindByNameAsync(command.Login.Email);

                if (user == null)
                {
                    logger.LogWarning("User not found.");
                    throw new Exception("Invalid credentials");
                }
            }

            logger.LogInformation("User found. Checking password.");

            var result = await signInManager.CheckPasswordSignInAsync(
                user,
                command.Login.Password,
                false);

            if (!result.Succeeded)
            {
                logger.LogWarning("Invalid password.");
                throw new Exception("Invalid credentials");
            }

            logger.LogInformation("Generating token.");

            var accessToken = await tokenGenerator.GenerateTokenAsync(user);

            logger.LogInformation("Creating refresh token.");

            var refreshToken = RefreshToken.Create(
                user.Id,
                Guid.NewGuid().ToString(),
                DateTime.UtcNow.AddDays(7),
                user.Email);

            dbContext.Set<RefreshToken>().Add(refreshToken);

            logger.LogInformation("Updating user.");

            await userManager.UpdateAsync(user);

            var activeToken = user.GetActiveRefreshToken(refreshToken.Token);

            logger.LogInformation("Active token found: {Found}", activeToken != null);

            return new LoginResult(new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = activeToken?.Token
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Login failed for {Email}", command.Login.Email);
            throw;
        }
    }
}