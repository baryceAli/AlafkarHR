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
    public bool IsMockedLocation { get; set; }
    public string? LocationIntegrityNote { get; set; }
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
    public bool IsMockedLocation { get; set; }
    public string? LocationIntegrityNote { get; set; }
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
    public bool IsMockedLocation { get; set; }
    public string? LocationIntegrityNote { get; set; }
    public string? Notes { get; set; }
}

public class AttendanceSessionDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public Guid? ShiftId { get; set; }
    public EmployeeAttendanceType AttendanceType { get; set; }
    public DateTime ShiftStart { get; set; }
    public DateTime ShiftEnd { get; set; }
    public DateTime? ActualStartTime { get; set; }
    public DateTime? ActualEndTime { get; set; }
    public AttendanceSessionStatus Status { get; set; }
    public AttendanceNormalizationStatus NormalizationStatus { get; set; } = AttendanceNormalizationStatus.Normal;
    public decimal TotalHours { get; set; }
    public decimal TotalDistanceKm { get; set; }
    public string? NormalizationNote { get; set; }
    public string? NormalizedBy { get; set; }
    public DateTime? NormalizedAt { get; set; }
    public bool RequiresNormalization => NormalizationStatus is not AttendanceNormalizationStatus.Normal;
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
    public bool IsMockedLocation { get; set; }
    public string? LocationIntegrityNote { get; set; }
    public bool IsBeforeShiftStart { get; set; }
    public bool IsLate { get; set; }
    public bool IsProhibitedByTime { get; set; }
    public bool IsAttendanceCompleted { get; set; }
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
    public bool IsMockedLocation { get; set; }
    public string? LocationIntegrityNote { get; set; }
}

public class ReviewLateCheckInRequestDto
{
    public Guid RequestId { get; set; }
    public bool IsApproved { get; set; }
    public DateTime? RegisteredCheckInTimeUtc { get; set; }
    public string? ManagerNote { get; set; }
}

public class EndMissingCheckInAttendanceSessionDto
{
    public Guid EmployeeId { get; set; }
    public Guid? ShiftId { get; set; }
    public DateTime ShiftStart { get; set; }
    public DateTime ShiftEnd { get; set; }
}

public class NormalizeAttendanceSessionDto
{
    public Guid SessionId { get; set; }
    public DateTime? CheckInTimeUtc { get; set; }
    public DateTime? CheckOutTimeUtc { get; set; }
    public bool MarkAbsent { get; set; }
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

public class AttendanceConfigurationDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public DayOfWeek FirstDayOfWeek { get; set; } = DayOfWeek.Saturday;
    public List<AttendanceDayScheduleDto> DaySchedules { get; set; } = AttendanceDayScheduleDto.DefaultWeek();
    public List<DayOfWeek> WeekendDays { get; set; } = [DayOfWeek.Friday, DayOfWeek.Saturday];
}

public class UpsertAttendanceConfigurationDto
{
    public Guid CompanyId { get; set; }
    public DayOfWeek FirstDayOfWeek { get; set; } = DayOfWeek.Saturday;
    public List<AttendanceDayScheduleDto> DaySchedules { get; set; } = AttendanceDayScheduleDto.DefaultWeek();
    public List<DayOfWeek> WeekendDays { get; set; } = [DayOfWeek.Friday, DayOfWeek.Saturday];
}

public class AttendanceDayScheduleDto
{
    public DayOfWeek DayOfWeek { get; set; }
    public bool IsWorkingDay { get; set; }
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }

    public static List<AttendanceDayScheduleDto> DefaultWeek()
        => Enum.GetValues<DayOfWeek>()
            .Select(day => new AttendanceDayScheduleDto
            {
                DayOfWeek = day,
                IsWorkingDay = day is not DayOfWeek.Friday and not DayOfWeek.Saturday,
                StartTime = day is DayOfWeek.Friday or DayOfWeek.Saturday ? null : new TimeSpan(8, 0, 0),
                EndTime = day is DayOfWeek.Friday or DayOfWeek.Saturday ? null : new TimeSpan(17, 0, 0)
            })
            .ToList();
}

public class AttendanceHolidayDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public AttendanceHolidayType HolidayType { get; set; } = AttendanceHolidayType.PublicHoliday;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsRecurringYearly { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Name { get; set; }
    public string? Description { get; set; }
}

public class UpsertAttendanceHolidayDto
{
    public Guid? Id { get; set; }
    public Guid CompanyId { get; set; }
    public AttendanceHolidayType HolidayType { get; set; } = AttendanceHolidayType.PublicHoliday;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsRecurringYearly { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Name { get; set; }
    public string? Description { get; set; }
}

public class AttendanceBreakPolicyDto
{
    public Guid Id { get; set; }
    public ShiftAssignmentScope Scope { get; set; }
    public Guid CompanyId { get; set; }
    public Guid? AdministrationId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? EmployeeId { get; set; }
    public bool IsEnabled { get; set; }
    public AttendanceBreakMode BreakMode { get; set; }
    public TimeSpan? BreakStartTime { get; set; }
    public TimeSpan? BreakEndTime { get; set; }
    public int AllowedDurationMinutes { get; set; }
    public bool IsPaid { get; set; }
}

public class UpsertAttendanceBreakPolicyDto
{
    public Guid? Id { get; set; }
    public ShiftAssignmentScope Scope { get; set; }
    public Guid CompanyId { get; set; }
    public Guid? AdministrationId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? EmployeeId { get; set; }
    public bool IsEnabled { get; set; } = true;
    public AttendanceBreakMode BreakMode { get; set; } = AttendanceBreakMode.Flexible;
    public TimeSpan? BreakStartTime { get; set; }
    public TimeSpan? BreakEndTime { get; set; }
    public int AllowedDurationMinutes { get; set; }
    public bool IsPaid { get; set; } = true;
}

public class EmergencyLeaveRequestDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid CompanyId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? AttachmentPath { get; set; }
    public AttendanceExceptionStatus Status { get; set; }
    public string? ApproverUserId { get; set; }
    public DateTime? ApprovalDateUtc { get; set; }
    public string? ApproverComment { get; set; }
}

public class CreateEmergencyLeaveRequestDto
{
    public Guid EmployeeId { get; set; }
    public Guid CompanyId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? AttachmentPath { get; set; }
}

public class ReviewEmergencyLeaveRequestDto
{
    public Guid RequestId { get; set; }
    public bool IsApproved { get; set; }
    public string? ApproverComment { get; set; }
}

public class EmployeeLeaveBalanceDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid CompanyId { get; set; }
    public int Year { get; set; } = DateTime.Today.Year;
    public decimal AnnualLeaveDays { get; set; }
    public bool AllowCarryForward { get; set; }
    public decimal MaxCarryForwardDays { get; set; }
    public decimal CarriedForwardDays { get; set; }
    public decimal TakenDays { get; set; }
    public decimal AvailableDays { get; set; }
    public decimal RemainingDays { get; set; }
}

public class UpsertEmployeeLeaveBalanceDto
{
    public Guid? Id { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid CompanyId { get; set; }
    public int Year { get; set; } = DateTime.Today.Year;
    public decimal AnnualLeaveDays { get; set; } = 30;
    public bool AllowCarryForward { get; set; } = true;
    public decimal MaxCarryForwardDays { get; set; } = 5;
}

public class LeaveReportFilterDto
{
    public Guid CompanyId { get; set; }
    public int Year { get; set; } = DateTime.Today.Year;
    public Guid? EmployeeId { get; set; }
    public AttendanceExceptionStatus? Status { get; set; }
}

public class LeaveReportRowDto
{
    public Guid EmployeeId { get; set; }
    public int Year { get; set; }
    public decimal AnnualLeaveDays { get; set; }
    public decimal CarriedForwardDays { get; set; }
    public decimal AvailableDays { get; set; }
    public decimal TakenDays { get; set; }
    public decimal RemainingDays { get; set; }
    public int PendingRequests { get; set; }
    public int ApprovedRequests { get; set; }
    public int RejectedRequests { get; set; }
}

public class LeaveReportDto
{
    public int Year { get; set; }
    public List<LeaveReportRowDto> Rows { get; set; } = [];
}

public class MidDayPermissionRequestDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid CompanyId { get; set; }
    public DateTime Date { get; set; }
    public DateTime RequestedStartUtc { get; set; }
    public DateTime RequestedEndUtc { get; set; }
    public DateTime? ApprovedStartUtc { get; set; }
    public DateTime? ApprovedEndUtc { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public AttendanceExceptionStatus Status { get; set; }
    public string? ApproverUserId { get; set; }
    public DateTime? ApprovalDateUtc { get; set; }
    public string? ApproverComment { get; set; }
}

public class CreateMidDayPermissionRequestDto
{
    public Guid EmployeeId { get; set; }
    public Guid CompanyId { get; set; }
    public DateTime Date { get; set; }
    public DateTime RequestedStartUtc { get; set; }
    public DateTime RequestedEndUtc { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class ReviewMidDayPermissionRequestDto
{
    public Guid RequestId { get; set; }
    public bool IsApproved { get; set; }
    public DateTime? ApprovedStartUtc { get; set; }
    public DateTime? ApprovedEndUtc { get; set; }
    public string? ApproverComment { get; set; }
}

public class AttendanceReportFilterDto
{
    public Guid CompanyId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public Guid? EmployeeId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? AdministrationId { get; set; }
    public AttendanceExceptionStatus? Status { get; set; }
    public string? Category { get; set; }
}

public class AttendanceReportRowDto
{
    public DateTime Date { get; set; }
    public Guid? EmployeeId { get; set; }
    public string Category { get; set; } = string.Empty;
    public AttendanceExceptionStatus? Status { get; set; }
    public AttendanceSessionStatus? SessionStatus { get; set; }
    public AttendanceNormalizationStatus? NormalizationStatus { get; set; }
    public DateTime? ShiftStartUtc { get; set; }
    public DateTime? ShiftEndUtc { get; set; }
    public DateTime? CheckInUtc { get; set; }
    public DateTime? CheckOutUtc { get; set; }
    public DateTime? RequestedStartUtc { get; set; }
    public DateTime? RequestedEndUtc { get; set; }
    public DateTime? ApprovedStartUtc { get; set; }
    public DateTime? ApprovedEndUtc { get; set; }
    public decimal TotalWorkingHours { get; set; }
    public decimal NetWorkingHours { get; set; }
    public int BreakMinutes { get; set; }
    public string? Reason { get; set; }
    public string? ApproverComment { get; set; }
    public string? NormalizationNote { get; set; }
    public bool RequiresNormalization => NormalizationStatus.HasValue
        && NormalizationStatus.Value is not AttendanceNormalizationStatus.Normal;
}

public class AttendanceReportDto
{
    public DayOfWeek FirstDayOfWeek { get; set; }
    public List<DayOfWeek> WeekendDays { get; set; } = [DayOfWeek.Friday, DayOfWeek.Saturday];
    public List<AttendanceReportRowDto> Rows { get; set; } = [];
}
