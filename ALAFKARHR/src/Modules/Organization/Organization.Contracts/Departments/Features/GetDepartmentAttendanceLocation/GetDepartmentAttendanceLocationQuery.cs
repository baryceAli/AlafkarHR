using Shared.Contracts.CQRS;

namespace Organization.Contracts.Departments.Features.GetDepartmentAttendanceLocation;

public record GetDepartmentAttendanceLocationQuery(Guid DepartmentId)
    : IQuery<GetDepartmentAttendanceLocationResult>;

public record GetDepartmentAttendanceLocationResult(
    Guid DepartmentId,
    Guid CompanyId,
    string Name,
    double Latitude,
    double Longitude,
    int AllowedRadiusMeters,
    bool IsActive);
