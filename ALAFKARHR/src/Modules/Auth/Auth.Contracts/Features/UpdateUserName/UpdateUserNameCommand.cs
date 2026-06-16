using Shared.Contracts.CQRS;

namespace Auth.Contracts.Features.UpdateUserName;

public record UpdateUserNameCommand(Guid CompanyId, string OldUserName, string NewUserName) : ICommand<UpdateUserNameResult>;

public record UpdateUserNameResult(bool IsSuccess);
