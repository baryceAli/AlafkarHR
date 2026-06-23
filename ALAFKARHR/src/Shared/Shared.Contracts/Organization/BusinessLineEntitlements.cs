namespace Shared.Contracts.Organization;

public record BusinessLineLicenseEntitlement(
    Guid ParentCompanyId,
    Guid CompanyLicenseId,
    Guid BusinessLineId,
    string BusinessLineKey,
    int ActivationLimit,
    int UsedActivations);

public interface IBusinessLineEntitlementService
{
    Task<bool> IsBusinessLineLicensedAsync(string businessLineKey, CancellationToken cancellationToken);
    Task<IReadOnlySet<string>> GetCurrentLicensedBusinessLineKeysAsync(CancellationToken cancellationToken);
    Task<BusinessLineLicenseEntitlement> GetEntitlementAsync(string businessLineKey, Guid companyId, int externalUsedActivations, CancellationToken cancellationToken);
    Task EnsureActivationAvailableAsync(string businessLineKey, Guid companyId, int externalUsedActivations, CancellationToken cancellationToken);
}
