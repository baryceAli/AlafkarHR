using Shared.Contracts.CQRS;
using SharedWithUI.Attendance.Enums;

namespace EmployeeModule.Contracts.Employees.Features.GetCompanyEmployeeRosterProfiles;

public record GetCompanyEmployeeRosterProfilesQuery(Guid CompanyId) : IQuery<GetCompanyEmployeeRosterProfilesResult>;

public record GetCompanyEmployeeRosterProfilesResult(List<EmployeeRosterProfileDto> Employees);

public record EmployeeRosterProfileDto(
    Guid EmployeeId,
    Guid CompanyId,
    Guid BranchId,
    Guid? AdministrationId,
    Guid? DepartmentId,
    Guid? PositionId,
    EmployeeAttendanceType AttendanceType,
    bool IsActive,
    string? EmployeeNo,
    string? Code,
    string? Email,
    string? FullName,
    string? FullNameEng,
    string? PositionName,
    string? PositionNameEng);
