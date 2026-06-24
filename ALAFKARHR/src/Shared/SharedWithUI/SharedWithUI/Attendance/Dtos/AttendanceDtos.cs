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

public enum AttendanceRosterStatus
{
    Draft,
    Published,
    Locked,
    Cancelled
}

public enum AttendanceImportBatchStatus
{
    Draft,
    Reviewed,
    Posted,
    Cancelled
}

public enum AttendanceImportRowStatus
{
    Pending,
    Accepted,
    Rejected,
    Posted
}

public enum AttendanceWorkEntryStatus
{
    Draft,
    Approved,
    Locked,
    Cancelled
}

public enum AttendanceWorkEntryType
{
    Regular,
    Overtime,
    PaidLeave,
    UnpaidLeave,
    Holiday,
    Absence,
    ManualCorrection
}

public class ShiftScheduleDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public AttendanceRosterStatus Status { get; set; }
    public string? Notes { get; set; }
    public int AssignmentCount { get; set; }
    public DateTime? PublishedAtUtc { get; set; }
    public DateTime? LockedAtUtc { get; set; }
}

public class UpsertShiftScheduleDto
{
    public Guid? Id { get; set; }
    public Guid CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; } = DateTime.Today;
    public DateTime EndDate { get; set; } = DateTime.Today;
    public string? Notes { get; set; }
}

public class ShiftScheduleAssignmentDto
{
    public Guid Id { get; set; }
    public Guid ScheduleId { get; set; }
    public Guid CompanyId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid ShiftId { get; set; }
    public DateTime WorkDate { get; set; }
    public string? EmployeeName { get; set; }
    public string? ShiftName { get; set; }
    public string? Notes { get; set; }
}

public class UpsertShiftScheduleAssignmentDto
{
    public Guid? Id { get; set; }
    public Guid ScheduleId { get; set; }
    public Guid CompanyId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid ShiftId { get; set; }
    public DateTime WorkDate { get; set; } = DateTime.Today;
    public string? Notes { get; set; }
}

public class BulkShiftScheduleAssignmentDto
{
    public Guid ScheduleId { get; set; }
    public Guid CompanyId { get; set; }
    public Guid ShiftId { get; set; }
    public List<Guid> EmployeeIds { get; set; } = [];
    public DateTime StartDate { get; set; } = DateTime.Today;
    public DateTime EndDate { get; set; } = DateTime.Today;
    public string? Notes { get; set; }
}

public class ShiftSwapRequestDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid? ScheduleAssignmentId { get; set; }
    public Guid RequestingEmployeeId { get; set; }
    public Guid TargetEmployeeId { get; set; }
    public DateTime WorkDate { get; set; }
    public Guid? RequestedShiftId { get; set; }
    public AttendanceExceptionStatus Status { get; set; }
    public string? Reason { get; set; }
    public string? ManagerNote { get; set; }
    public DateTime? ReviewedAtUtc { get; set; }
    public string? RequestingEmployeeName { get; set; }
    public string? TargetEmployeeName { get; set; }
    public string? RequestedShiftName { get; set; }
}

public class CreateShiftSwapRequestDto
{
    public Guid CompanyId { get; set; }
    public Guid? ScheduleAssignmentId { get; set; }
    public Guid RequestingEmployeeId { get; set; }
    public Guid TargetEmployeeId { get; set; }
    public DateTime WorkDate { get; set; } = DateTime.Today;
    public Guid? RequestedShiftId { get; set; }
    public string? Reason { get; set; }
}

public class ReviewShiftSwapRequestDto
{
    public Guid RequestId { get; set; }
    public bool IsApproved { get; set; }
    public string? ManagerNote { get; set; }
}

public class AttendanceCorrectionDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid? SessionId { get; set; }
    public DateTime WorkDate { get; set; }
    public DateTime? CorrectedCheckInUtc { get; set; }
    public DateTime? CorrectedCheckOutUtc { get; set; }
    public AttendanceExceptionStatus Status { get; set; }
    public string? Reason { get; set; }
    public string? ManagerNote { get; set; }
    public DateTime? ReviewedAtUtc { get; set; }
    public DateTime? AppliedAtUtc { get; set; }
    public string? EmployeeName { get; set; }
}

public class CreateAttendanceCorrectionDto
{
    public Guid CompanyId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid? SessionId { get; set; }
    public DateTime WorkDate { get; set; } = DateTime.Today;
    public DateTime? CorrectedCheckInUtc { get; set; }
    public DateTime? CorrectedCheckOutUtc { get; set; }
    public string? Reason { get; set; }
}

public class ReviewAttendanceCorrectionDto
{
    public Guid CorrectionId { get; set; }
    public bool IsApproved { get; set; }
    public string? ManagerNote { get; set; }
}

public class BiometricImportBatchDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string SourceName { get; set; } = string.Empty;
    public DateTime ImportedAtUtc { get; set; }
    public AttendanceImportBatchStatus Status { get; set; }
    public int TotalRows { get; set; }
    public int AcceptedRows { get; set; }
    public int RejectedRows { get; set; }
    public string? Notes { get; set; }
    public List<BiometricImportRowDto> Rows { get; set; } = [];
}

public class CreateBiometricImportBatchDto
{
    public Guid CompanyId { get; set; }
    public string SourceName { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class BiometricImportRowDto
{
    public Guid Id { get; set; }
    public Guid BatchId { get; set; }
    public Guid CompanyId { get; set; }
    public Guid? EmployeeId { get; set; }
    public string? DeviceEmployeeCode { get; set; }
    public DateTime PunchTimeUtc { get; set; }
    public bool IsCheckOut { get; set; }
    public AttendanceImportRowStatus Status { get; set; }
    public string? ErrorMessage { get; set; }
    public string? EmployeeName { get; set; }
}

public class UpsertBiometricImportRowDto
{
    public Guid? Id { get; set; }
    public Guid BatchId { get; set; }
    public Guid CompanyId { get; set; }
    public Guid? EmployeeId { get; set; }
    public string? DeviceEmployeeCode { get; set; }
    public DateTime PunchTimeUtc { get; set; } = DateTime.UtcNow;
    public bool IsCheckOut { get; set; }
    public string? ErrorMessage { get; set; }
}

public class ReviewBiometricImportRowDto
{
    public Guid RowId { get; set; }
    public bool IsAccepted { get; set; }
    public string? ErrorMessage { get; set; }
}

public class PayrollWorkEntryDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid EmployeeId { get; set; }
    public DateTime WorkDate { get; set; }
    public AttendanceWorkEntryType EntryType { get; set; }
    public decimal Hours { get; set; }
    public AttendanceWorkEntryStatus Status { get; set; }
    public Guid? SourceDocumentId { get; set; }
    public string? SourceModule { get; set; }
    public string? Notes { get; set; }
    public string? EmployeeName { get; set; }
    public DateTime? ApprovedAtUtc { get; set; }
    public DateTime? LockedAtUtc { get; set; }
}

public class GenerateAttendanceWorkEntriesDto
{
    public Guid CompanyId { get; set; }
    public Guid? EmployeeId { get; set; }
    public DateTime FromDate { get; set; } = DateTime.Today;
    public DateTime ToDate { get; set; } = DateTime.Today;
}

public class UpsertAttendanceWorkEntryDto
{
    public Guid? Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid EmployeeId { get; set; }
    public DateTime WorkDate { get; set; } = DateTime.Today;
    public AttendanceWorkEntryType EntryType { get; set; } = AttendanceWorkEntryType.ManualCorrection;
    public decimal Hours { get; set; }
    public Guid? SourceDocumentId { get; set; }
    public string? SourceModule { get; set; }
    public string? Notes { get; set; }
}
