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
