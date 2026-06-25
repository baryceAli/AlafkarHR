using Shared.Contracts.CQRS;
using SharedWithUI.Attendance.Enums;

namespace EmployeeModule.Contracts.Employees.Features.GetEmployeeAttendanceProfile;

public record GetEmployeeAttendanceProfileQuery(Guid EmployeeId) : IQuery<GetEmployeeAttendanceProfileResult>;
public record GetEmployeeAttendanceProfileByCodeQuery(string Code) : IQuery<GetEmployeeAttendanceProfileResult>;

public record GetEmployeeAttendanceProfileResult(
    Guid EmployeeId,
    Guid CompanyId,
    Guid BranchId,
    Guid? AdministrationId,
    Guid? DepartmentId,
    EmployeeAttendanceType AttendanceType,
    int? AllowedRadiusMeters,
    bool IsActive,
    string? Code = null,
    string? Email = null,
    string? FullName = null);
