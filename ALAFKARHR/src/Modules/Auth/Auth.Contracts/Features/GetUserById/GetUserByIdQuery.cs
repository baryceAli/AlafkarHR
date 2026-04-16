using Auth.Contracts.Dtos;
using Shared.Contracts.CQRS;

namespace Auth.Contracts.Features.GetUserById;

public record GetUserByIdQuery(Guid Id) : IQuery<GetUserByIdResult>;
public record GetUserByIdResult(UserDto User);