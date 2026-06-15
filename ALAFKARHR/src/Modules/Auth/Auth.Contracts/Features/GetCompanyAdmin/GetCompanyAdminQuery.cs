using Shared.Contracts.CQRS;

namespace Auth.Contracts.Features.GetCompanyAdmin;

public record GetCompanyAdminQuery(Guid CompanyId) : IQuery<GetCompanyAdminResult>;

public record GetCompanyAdminResult(Guid UserId, string UserName, string? Email, string? PhoneNumber);
