using Shared.Contracts.CQRS;
using SharedWithUI.Auth.Dtos;

namespace Auth.Contracts.Features.GetUserById;

public record GetUserByIdQuery(Guid Id) : IQuery<GetUserByIdResult>;
public record GetUserByIdResult(UserDto User);