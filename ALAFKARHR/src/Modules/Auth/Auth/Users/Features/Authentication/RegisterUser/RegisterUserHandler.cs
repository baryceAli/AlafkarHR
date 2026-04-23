using Auth.Contracts.Features.RegisterUser;

namespace Auth.Users.Features.Authentication.RegisterUser;


public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Register.UserName).NotEmpty().WithMessage("UserName is required");
        RuleFor(x => x.Register.Email).NotEmpty().WithMessage("Email is required");
        RuleFor(x => x.Register.PhoneNumber).NotEmpty().WithMessage("PhoneNumber is required");
        //RuleFor(x => x.Register.UserType).NotEmpty().WithMessage("UserType is required");
        RuleFor(x => x.Register.UserType).IsInEnum().WithMessage("UserType must be SystemUser, Driver, or Customer");
    }
}
public class RegisterUserHandler(UserManager<ApplicationUser> userManager, IMessageSender emailSender, IOptions<OTPOptions> oTPOptions)
    : ICommandHandler<RegisterUserCommand, RegisterUserResult>
{
    public async Task<RegisterUserResult> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        //var GenerateOTP = new GenerateOTP(oTPOptions);
        //var user = command.Register.Adapt<ApplicationUser>();
        var userToCreate = ApplicationUser.Create(
            Guid.NewGuid(),
            command.Register.UserName,
            command.Register.Email,
            command.Register.PhoneNumber,
            command.Register.UserType,
            GenerateOTP.Generate(oTPOptions.Value.Length),
            OTPType.ConfirmEmail,
            DateTime.UtcNow.AddMinutes(oTPOptions.Value.ExpirationMinutes),
            command.Register.CompanyId
            );
        
        var result = await userManager.CreateAsync(userToCreate, command.Register.Password);
        
        if (!result.Succeeded)
            throw new Exception(string.Join("Register user, ", result.Errors.Select(e => e.Description)));


        var registeredUser = await userManager.FindByNameAsync(command.Register.UserName);

        //send it to email
            await emailSender.SendAsync(registeredUser!.Email!, "Confirm your email", $"{registeredUser.Otp}");

        //Generate Token
        //var token =await userManager.GenerateEmailConfirmationTokenAsync(registeredUser);
        return new RegisterUserResult(registeredUser.Id);





    }
}
