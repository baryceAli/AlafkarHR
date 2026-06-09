using SharedWithUI.Attendance.Enums;

namespace SharedWithUI.Attendance.Dtos;

public class StartAttendanceSessionDto
{
    public Guid EmployeeId { get; set; }
    public Guid? ShiftId { get; set; }
    public DateTime ShiftStart { get; set; }
    public DateTime ShiftEnd { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double? AccuracyMeters { get; set; }
    public string? ManualOverrideReason { get; set; }
}

public class EndAttendanceSessionDto
{
    public Guid SessionId { get; set; }
}

public class AttendanceLocationPingDto
{
    public Guid? ClientPingId { get; set; }
    public Guid SessionId { get; set; }
    public Guid EmployeeId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double? AccuracyMeters { get; set; }
    public DateTime RecordedAtUtc { get; set; }
}

public class AttendanceCheckInDto
{
    public Guid? ClientCheckInId { get; set; }
    public Guid SessionId { get; set; }
    public Guid EmployeeId { get; set; }
    public string SiteName { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime ArrivedAtUtc { get; set; }
    public DateTime? DepartedAtUtc { get; set; }
    public string? Notes { get; set; }
}

public class AttendanceSessionDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid? ShiftId { get; set; }
    public EmployeeAttendanceType AttendanceType { get; set; }
    public DateTime ShiftStart { get; set; }
    public DateTime ShiftEnd { get; set; }
    public DateTime? ActualStartTime { get; set; }
    public DateTime? ActualEndTime { get; set; }
    public AttendanceSessionStatus Status { get; set; }
    public decimal TotalHours { get; set; }
    public decimal TotalDistanceKm { get; set; }
}

public class AttendanceCheckInPreviewDto
{
    public Guid EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public string? EmployeeCode { get; set; }
    public string? EmployeeEmail { get; set; }
    public Guid? ShiftId { get; set; }
    public string? ShiftName { get; set; }
    public EmployeeAttendanceType AttendanceType { get; set; }
    public DateTime? ShiftStart { get; set; }
    public DateTime? ShiftEnd { get; set; }
    public DateTime? LateAfterUtc { get; set; }
    public DateTime? ProhibitCheckInAfterUtc { get; set; }
    public AttendanceSessionDto? ActiveSession { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double? AccuracyMeters { get; set; }
    public double? DistanceMeters { get; set; }
    public double? AllowedRadiusMeters { get; set; }
    public bool HasLocation { get; set; }
    public bool IsWithinAllowedRadius { get; set; }
    public bool IsBeforeShiftStart { get; set; }
    public bool IsLate { get; set; }
    public bool IsProhibitedByTime { get; set; }
    public bool CanCheckIn { get; set; }
    public bool CanSubmitLateRequest { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class CreateLateCheckInRequestDto
{
    public Guid EmployeeId { get; set; }
    public Guid? ShiftId { get; set; }
    public DateTime ShiftStart { get; set; }
    public DateTime ShiftEnd { get; set; }
    public DateTime RequestedCheckInTimeUtc { get; set; }
    public string Reason { get; set; } = string.Empty;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double? AccuracyMeters { get; set; }
}

public class ReviewLateCheckInRequestDto
{
    public Guid RequestId { get; set; }
    public bool IsApproved { get; set; }
    public DateTime? RegisteredCheckInTimeUtc { get; set; }
    public string? ManagerNote { get; set; }
}

public class CreateShiftDto
{
    public string Name { get; set; } = string.Empty;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int GracePeriodMinutes { get; set; } = 15;
    public int LateAfterMinutes { get; set; } = 15;
    public int ProhibitCheckInAfterMinutes { get; set; } = 120;
    public int BreakMinutes { get; set; }
    public Guid CompanyId { get; set; }
}

public class LateCheckInRequestDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid? ShiftId { get; set; }
    public Guid? SessionId { get; set; }
    public EmployeeAttendanceType AttendanceType { get; set; }
    public DateTime ShiftStart { get; set; }
    public DateTime ShiftEnd { get; set; }
    public DateTime RequestedCheckInTimeUtc { get; set; }
    public DateTime? RegisteredCheckInTimeUtc { get; set; }
    public string Reason { get; set; } = string.Empty;
    public AttendanceExceptionStatus Status { get; set; }
    public string? ManagerNote { get; set; }
    public DateTime? ReviewedAt { get; set; }
}

public class ShiftDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int GracePeriodMinutes { get; set; }
    public int LateAfterMinutes { get; set; }
    public int ProhibitCheckInAfterMinutes { get; set; }
    public int BreakMinutes { get; set; }
    public bool IsFlexible { get; set; }
    public Guid CompanyId { get; set; }
}

public class AttendanceDashboardDto
{
    public int ActiveSessions { get; set; }
    public int OnBreakSessions { get; set; }
    public int CompletedToday { get; set; }
    public int PendingLateCheckInRequests { get; set; }
    public int FixedLocationSessionsToday { get; set; }
    public int MobileSessionsToday { get; set; }
    public List<AttendanceSessionDto> RecentSessions { get; set; } = [];
    public List<LateCheckInRequestDto> PendingRequests { get; set; } = [];
}

public class AssignShiftDto
{
    public Guid ShiftId { get; set; }
    public ShiftAssignmentScope Scope { get; set; }
    public Guid CompanyId { get; set; }
    public Guid? AdministrationId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? EmployeeId { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}

public class ShiftAssignmentDto
{
    public Guid Id { get; set; }
    public Guid ShiftId { get; set; }
    public ShiftAssignmentScope Scope { get; set; }
    public Guid CompanyId { get; set; }
    public Guid? AdministrationId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? EmployeeId { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public bool IsActive { get; set; }
}
