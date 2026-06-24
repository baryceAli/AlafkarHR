using Shared.Contracts.CQRS;

namespace Shared.Contracts.StoreFront;

public record GetStoreFrontBranchScopeQuery(Guid StoreFrontId) : IQuery<GetStoreFrontBranchScopeResult>;

public record GetStoreFrontBranchScopeResult(Guid StoreFrontId, Guid CompanyId, Guid BranchId);
