using Shared.Contracts.CQRS;

namespace Auth.Contracts.Features.UserCompanyMembership;

public record UserBelongsToCompanyQuery(Guid UserId, Guid CompanyId) : IQuery<UserBelongsToCompanyResult>;

public record UserBelongsToCompanyResult(bool BelongsToCompany);
