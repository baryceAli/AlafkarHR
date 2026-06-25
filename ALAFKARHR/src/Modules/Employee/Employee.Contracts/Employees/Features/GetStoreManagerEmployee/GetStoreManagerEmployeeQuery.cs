using Shared.Contracts.CQRS;

namespace EmployeeModule.Contracts.Employees.Features.GetStoreManagerEmployee;

public record GetStoreManagerEmployeeQuery(Guid CompanyId, Guid EmployeeId) : IQuery<GetStoreManagerEmployeeResult>;

public record GetStoreManagerEmployeeResult(
    Guid EmployeeId,
    Guid CompanyId,
    Guid LinkedUserId,
    string FullName,
    string FullNameEng,
    bool IsActive);
