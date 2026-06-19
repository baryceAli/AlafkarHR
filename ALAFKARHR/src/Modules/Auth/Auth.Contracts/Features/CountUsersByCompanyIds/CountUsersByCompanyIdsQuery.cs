using Shared.Contracts.CQRS;

namespace Auth.Contracts.Features.CountUsersByCompanyIds;

public record CountUsersByCompanyIdsQuery(IReadOnlyCollection<Guid> CompanyIds) : IQuery<CountUsersByCompanyIdsResult>;

public record CountUsersByCompanyIdsResult(int Count);
