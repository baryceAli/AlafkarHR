using Shared.Contracts.CQRS;
using SharedWithUI.Auth.Dtos;

namespace Auth.Contracts.Features.RegisterUser;

public record RegisterUserCommand(RegisterDto Register) : ICommand<RegisterUserResult>;
public record RegisterUserResult(Guid Id);

