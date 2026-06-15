using Shared.Contracts.CQRS;

namespace Organization.Contracts.Companies.Features.GetCompanyAccessStatus;

public record GetCompanyAccessStatusQuery(Guid CompanyId) : IQuery<GetCompanyAccessStatusResult>;

public record GetCompanyAccessStatusResult(bool CanLogin);
