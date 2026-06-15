using Shared.Contracts.CQRS;

namespace Auth.Contracts.Features.CreateCompanyAdmin;

public record CreateCompanyAdminCommand(
    Guid CompanyId,
    string CompanyCode,
    string UserName,
    string Email,
    string PhoneNumber,
    string TemporaryPassword) : ICommand<CreateCompanyAdminResult>;

public record CreateCompanyAdminResult(Guid UserId, string RoleName);
