using Auth.Helpers;
using Microsoft.Extensions.Logging;
using Organization.Contracts.Companies.Features.GetCompanyAccessStatus;

namespace Auth.Users.Features.Authentication.Login;

public record LoginCommand(LoginDto Login) : ICommand<LoginResult>;

public record LoginResult(LoginResponseDto Login);

public class LoginHandler(
    AuthDbContext dbContext,
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    ILogger<LoginHandler> logger,
    IJwtTokenGenerator tokenGenerator,
    ISender sender)
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

            if (user.CompanyId.HasValue)
            {
                var accessStatus = await sender.Send(new GetCompanyAccessStatusQuery(user.CompanyId.Value), cancellationToken);
                if (!accessStatus.CanLogin)
                {
                    logger.LogWarning("Login blocked for inactive company {CompanyId}", user.CompanyId);
                    throw new Exception("Company is disabled");
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

            var rawRefreshToken = RefreshTokenGenerator.Generate();
            var refreshTokenHash = RefreshTokenGenerator.Hash(rawRefreshToken);

            var refreshToken = RefreshToken.Create(
                user.Id,
                refreshTokenHash,
                DateTime.UtcNow.AddDays(7),
                user.Email ?? user.UserName ?? user.Id.ToString());

            dbContext.Set<RefreshToken>().Add(refreshToken);

            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Refresh token created.");

            return new LoginResult(new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = rawRefreshToken
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Login failed for {Email}", command.Login.Email);
            throw;
        }
    }
}
