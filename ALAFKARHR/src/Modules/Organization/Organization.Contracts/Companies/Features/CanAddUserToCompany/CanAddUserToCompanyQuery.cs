using Shared.Contracts.CQRS;

namespace Organization.Contracts.Companies.Features.CanAddUserToCompany;

public record CanAddUserToCompanyQuery(Guid CompanyId) : IQuery<CanAddUserToCompanyResult>;

public record CanAddUserToCompanyResult(bool CanAdd, string? Reason);
