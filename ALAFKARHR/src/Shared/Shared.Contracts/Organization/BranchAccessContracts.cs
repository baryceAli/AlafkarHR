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

public record GetCompanyUserBranchAssignmentsQuery(Guid CompanyId) : IQuery<GetCompanyUserBranchAssignmentsResult>;

public record GetCompanyUserBranchAssignmentsResult(List<UserBranchAssignmentInfo> Assignments);

public record UserBranchAssignmentInfo(Guid UserId, Guid CompanyId, Guid BranchId, bool IsDefault);

public record EnsureUserBranchAccessCommand(Guid UserId, Guid CompanyId, Guid BranchId, bool MakeDefault)
    : ICommand<EnsureUserBranchAccessResult>;

public record EnsureUserBranchAccessResult(bool IsSuccess);

public record AssignUserBranchesCommand(Guid UserId, Guid CompanyId, List<Guid> BranchIds, Guid? DefaultBranchId)
    : ICommand<AssignUserBranchesResult>;

public record AssignUserBranchesResult(int AssignedCount);

public record GetCompanyBranchRoleAssignmentsForDashboardQuery(Guid CompanyId) : IQuery<GetCompanyBranchRoleAssignmentsForDashboardResult>;

public record GetCompanyBranchRoleAssignmentsForDashboardResult(List<BranchRoleAssignmentInfo> Assignments);

public record BranchRoleAssignmentInfo(Guid Id, Guid UserId, Guid CompanyId, Guid BranchId, string TemplateKey, DateTime? CreatedAt);

public record EnsureStoreFrontBranchCommand(
    Guid CompanyId,
    Guid? BranchId,
    string Name,
    string NameEng,
    string Code,
    string? Phone,
    string? Email,
    string UserId) : ICommand<EnsureStoreFrontBranchResult>;

public record EnsureStoreFrontBranchResult(Guid BranchId);

public record GetBranchScopeInfoQuery(Guid CompanyId, Guid BranchId) : IQuery<GetBranchScopeInfoResult>;

public record GetBranchScopeInfoResult(Guid BranchId, Guid CompanyId, int Specialization);

public record EnsureCurrentUserBranchPermissionQuery(Guid CompanyId, Guid BranchId, string Permission) : IQuery<EnsureCurrentUserBranchPermissionResult>;

public record EnsureCurrentUserBranchPermissionResult(bool HasPermission);

public record GetCurrentUserBranchRolePermissionsQuery(Guid CompanyId, Guid? UserId = null) : IQuery<GetCurrentUserBranchRolePermissionsResult>;

public record GetCurrentUserBranchRolePermissionsResult(List<string> Permissions);

public record GetCurrentUserBranchRoleAccessForAuthorizationQuery(Guid CompanyId) : IQuery<GetCurrentUserBranchRoleAccessForAuthorizationResult>;

public record GetCurrentUserBranchRoleAccessForAuthorizationResult(List<BranchRolePermissionAccess> Assignments);

public record BranchRolePermissionAccess(Guid BranchId, List<string> Permissions);

public record AssignStoreFrontBranchRoleCommand(Guid UserId, Guid CompanyId, Guid BranchId, string TemplateKey)
    : ICommand<AssignStoreFrontBranchRoleResult>;

public record AssignStoreFrontBranchRoleResult(Guid Id);

public record RevokeStoreFrontBranchRoleCommand(Guid UserId, Guid CompanyId, Guid BranchId, string TemplateKey)
    : ICommand<RevokeStoreFrontBranchRoleResult>;

public record RevokeStoreFrontBranchRoleResult(bool IsSuccess);

public record BranchAccountingInfo(
    Guid BranchId,
    Guid CompanyId,
    string Code,
    string Name,
    string NameEng,
    bool IsMainBranch);
