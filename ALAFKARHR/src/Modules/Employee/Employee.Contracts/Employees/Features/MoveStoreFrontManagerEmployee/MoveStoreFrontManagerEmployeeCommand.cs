using Shared.Contracts.CQRS;

namespace EmployeeModule.Contracts.Employees.Features.MoveStoreFrontManagerEmployee;

public record MoveStoreFrontManagerEmployeeCommand(
    Guid CompanyId,
    Guid EmployeeId,
    Guid BranchId,
    string UserId,
    bool RequireActive,
    bool RequireLinkedUser) : ICommand<MoveStoreFrontManagerEmployeeResult>;

public record MoveStoreFrontManagerEmployeeResult(
    Guid EmployeeId,
    Guid CompanyId,
    Guid BranchId,
    Guid? LinkedUserId,
    string FullName,
    string FullNameEng,
    bool IsActive);
