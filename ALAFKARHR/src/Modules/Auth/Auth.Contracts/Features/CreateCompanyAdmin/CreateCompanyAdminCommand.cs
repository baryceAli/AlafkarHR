using Shared.Contracts.CQRS;

namespace Auth.Contracts.Features.CreateCompanyAdmin;

public enum CompanyAdminScope
{
    TenantOperations = 0,
    ParentCompanyAdministration = 1
}

public record CreateCompanyAdminCommand(
    Guid CompanyId,
    string CompanyCode,
    string UserName,
    string Email,
    string PhoneNumber,
    string TemporaryPassword,
    CompanyAdminScope AdminScope = CompanyAdminScope.TenantOperations) : ICommand<CreateCompanyAdminResult>;

public record CreateCompanyAdminResult(Guid UserId, string RoleName);
