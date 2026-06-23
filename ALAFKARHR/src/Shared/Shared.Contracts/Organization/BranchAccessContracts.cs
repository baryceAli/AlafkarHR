using Shared.Contracts.CQRS;

namespace Shared.Contracts.Organization;

public record EnsureMainBranchCommand(Guid CompanyId, string UserId) : ICommand<EnsureMainBranchResult>;

public record EnsureMainBranchResult(Guid BranchId);

public record GetCurrentUserBranchAccessQuery(Guid CompanyId) : IQuery<GetCurrentUserBranchAccessResult>;

public record GetCurrentUserBranchAccessResult(bool CanViewAllBranches, List<Guid> BranchIds);

public record GetCompanyBranchesForAccountingQuery(Guid CompanyId) : IQuery<GetCompanyBranchesForAccountingResult>;

public record GetCompanyBranchesForAccountingResult(List<BranchAccountingInfo> Branches);

public record GetUserBranchAssignmentsQuery(Guid UserId, Guid CompanyId) : IQuery<GetUserBranchAssignmentsResult>;

public record GetUserBranchAssignmentsResult(List<Guid> BranchIds, Guid? DefaultBranchId);

public record BranchAccountingInfo(
    Guid BranchId,
    Guid CompanyId,
    string Code,
    string Name,
    string NameEng,
    bool IsMainBranch);
