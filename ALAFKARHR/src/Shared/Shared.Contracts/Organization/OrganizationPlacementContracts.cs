using Shared.Contracts.CQRS;

namespace Shared.Contracts.Organization;

public record ValidateOrganizationPlacementQuery(
    Guid CompanyId,
    Guid? BranchId,
    Guid? AdministrationId,
    Guid? DepartmentId) : IQuery<ValidateOrganizationPlacementResult>;

public record ValidateOrganizationPlacementResult(bool IsValid, string? Message);
