using Shared.Contracts.CQRS;
using SharedWithUI.Auth.Dtos;

namespace Auth.Contracts.Features.GetByUserName;

public record GetByUserNameQuery(string UserName) : IQuery<GetByUserNameResult>;
public record GetByUserNameResult(UserDto User);
