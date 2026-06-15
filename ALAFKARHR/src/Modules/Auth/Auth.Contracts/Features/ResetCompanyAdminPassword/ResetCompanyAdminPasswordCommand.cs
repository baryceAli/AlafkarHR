using Shared.Contracts.CQRS;

namespace Auth.Contracts.Features.ResetCompanyAdminPassword;

public record ResetCompanyAdminPasswordCommand(Guid CompanyId, string TemporaryPassword) : ICommand<ResetCompanyAdminPasswordResult>;

public record ResetCompanyAdminPasswordResult(bool IsSuccess);
