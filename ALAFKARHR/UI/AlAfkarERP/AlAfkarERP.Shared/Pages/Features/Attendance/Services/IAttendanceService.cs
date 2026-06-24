using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Attendance.Dtos;
using SharedWithUI.Attendance.Enums;

namespace AlAfkarERP.Shared.Pages.Features.Attendance.Services;

public interface IAttendanceService
{
    Task<ApiResult<AttendanceDashboardDto>> GetDashboardAsync(Guid? employeeId = null);
    Task<ApiResult<PaginatedResult<AttendanceSessionDto>>> GetSessionsAsync(int pageIndex, int pageSize, Guid? employeeId = null, DateTime? fromUtc = null, DateTime? toUtc = null);
    Task<ApiResult<List<ShiftDto>>> GetShiftsAsync(Guid? companyId = null);
    Task<ApiResult<ShiftDto>> CreateShiftAsync(CreateShiftDto shift);
    Task<ApiResult<ShiftDto>> UpdateShiftAsync(ShiftDto shift);
    Task<ApiResult<bool>> DeleteShiftAsync(Guid shiftId);
    Task<ApiResult<AttendanceCheckInPreviewDto>> GetCheckInPreviewAsync(Guid employeeId, double? latitude = null, double? longitude = null, double? accuracyMeters = null, bool isMockedLocation = false, string? locationIntegrityNote = null, DateTime? workDateUtc = null);
    Task<ApiResult<AttendanceCheckInPreviewDto>> GetMyCheckInPreviewAsync(double? latitude = null, double? longitude = null, double? accuracyMeters = null, bool isMockedLocation = false, string? locationIntegrityNote = null, DateTime? workDateUtc = null);
    Task<ApiResult<PaginatedResult<ShiftAssignmentDto>>> GetShiftAssignmentsAsync(int pageIndex, int pageSize, Guid? companyId = null, ShiftAssignmentScope? scope = null);
    Task<ApiResult<ShiftAssignmentDto>> AssignShiftAsync(AssignShiftDto assignment);
    Task<ApiResult<PaginatedResult<LateCheckInRequestDto>>> GetLateCheckInRequestsAsync(int pageIndex, int pageSize, AttendanceExceptionStatus? status = null, Guid? employeeId = null);
    Task<ApiResult<AttendanceSessionDto>> StartSessionAsync(StartAttendanceSessionDto session);
    Task<ApiResult<AttendanceSessionDto>> EndSessionAsync(Guid sessionId);
    Task<ApiResult<AttendanceSessionDto>> EndMissingCheckInSessionAsync(EndMissingCheckInAttendanceSessionDto session);
    Task<ApiResult<AttendanceSessionDto>> NormalizeSessionAsync(NormalizeAttendanceSessionDto session);
    Task<ApiResult<AttendanceSessionDto>> StartBreakAsync(Guid sessionId);
    Task<ApiResult<AttendanceSessionDto>> EndBreakAsync(Guid sessionId);
    Task<ApiResult<bool>> SubmitLocationPingAsync(AttendanceLocationPingDto ping);
    Task<ApiResult<bool>> CreateCheckInAsync(AttendanceCheckInDto checkIn);
    Task<ApiResult<LateCheckInRequestDto>> CreateLateCheckInRequestAsync(CreateLateCheckInRequestDto request);
    Task<ApiResult<LateCheckInReviewResultDto>> ReviewLateCheckInRequestAsync(ReviewLateCheckInRequestDto review);
    Task<ApiResult<AttendanceCalendarSettingsDto>> GetCalendarSettingsAsync(Guid companyId);
    Task<ApiResult<AttendanceCalendarSettingsDto>> UpsertCalendarSettingsAsync(UpsertAttendanceCalendarSettingsDto settings);
    Task<ApiResult<List<AttendanceHolidayDto>>> GetHolidaysAsync(Guid companyId, DateTime? fromDate = null, DateTime? toDate = null);
    Task<ApiResult<AttendanceHolidayDto>> UpsertHolidayAsync(UpsertAttendanceHolidayDto holiday);
    Task<ApiResult<bool>> DeleteHolidayAsync(Guid holidayId);
    Task<ApiResult<PaginatedResult<MidDayPermissionRequestDto>>> GetMidDayPermissionsAsync(Guid companyId, int pageIndex, int pageSize, AttendanceExceptionStatus? status = null, Guid? employeeId = null);
    Task<ApiResult<MidDayPermissionRequestDto>> CreateMidDayPermissionAsync(CreateMidDayPermissionRequestDto request);
    Task<ApiResult<MidDayPermissionRequestDto>> ReviewMidDayPermissionAsync(ReviewMidDayPermissionRequestDto review);
    Task<ApiResult<AttendanceReportDto>> GetReportAsync(AttendanceReportFilterDto filter);
    Task<ApiResult<AttendanceRosterControlDto>> GetRosterControlAsync(AttendanceRosterControlFilterDto filter);
    Task<ApiResult<List<AttendanceRosterSubstituteConfigurationDto>>> GetRosterSubstituteConfigurationsAsync(Guid companyId);
    Task<ApiResult<AttendanceRosterSubstituteConfigurationDto>> UpsertRosterSubstituteConfigurationAsync(UpsertAttendanceRosterSubstituteConfigurationDto configuration);
    Task<ApiResult<List<ShiftScheduleDto>>> GetShiftSchedulesAsync(Guid companyId, AttendanceRosterStatus? status = null);
    Task<ApiResult<ShiftScheduleDto>> UpsertShiftScheduleAsync(UpsertShiftScheduleDto schedule);
    Task<ApiResult<ShiftScheduleDto>> PublishShiftScheduleAsync(Guid scheduleId);
    Task<ApiResult<ShiftScheduleDto>> LockShiftScheduleAsync(Guid scheduleId);
    Task<ApiResult<ShiftScheduleDto>> CancelShiftScheduleAsync(Guid scheduleId);
    Task<ApiResult<PaginatedResult<ShiftScheduleAssignmentDto>>> GetShiftScheduleAssignmentsAsync(int pageIndex, int pageSize, Guid? scheduleId = null, Guid? employeeId = null, DateTime? fromDate = null, DateTime? toDate = null);
    Task<ApiResult<ShiftScheduleAssignmentDto>> UpsertShiftScheduleAssignmentAsync(UpsertShiftScheduleAssignmentDto assignment);
    Task<ApiResult<int>> BulkShiftScheduleAssignmentAsync(BulkShiftScheduleAssignmentDto assignment);
    Task<ApiResult<bool>> DeleteShiftScheduleAssignmentAsync(Guid assignmentId);
    Task<ApiResult<List<ShiftSwapRequestDto>>> GetShiftSwapRequestsAsync(Guid companyId, AttendanceExceptionStatus? status = null, Guid? employeeId = null);
    Task<ApiResult<ShiftSwapRequestDto>> CreateShiftSwapRequestAsync(CreateShiftSwapRequestDto request);
    Task<ApiResult<ShiftSwapRequestDto>> ReviewShiftSwapRequestAsync(ReviewShiftSwapRequestDto review);
    Task<ApiResult<ShiftSwapRequestDto>> CancelShiftSwapRequestAsync(Guid requestId);
    Task<ApiResult<List<AttendanceCorrectionDto>>> GetAttendanceCorrectionsAsync(Guid companyId, AttendanceExceptionStatus? status = null, Guid? employeeId = null);
    Task<ApiResult<AttendanceCorrectionDto>> CreateAttendanceCorrectionAsync(CreateAttendanceCorrectionDto correction);
    Task<ApiResult<AttendanceCorrectionDto>> ReviewAttendanceCorrectionAsync(ReviewAttendanceCorrectionDto review);
    Task<ApiResult<AttendanceCorrectionDto>> ApplyAttendanceCorrectionAsync(Guid correctionId);
    Task<ApiResult<List<BiometricImportBatchDto>>> GetBiometricImportBatchesAsync(Guid companyId);
    Task<ApiResult<BiometricImportBatchDto>> CreateBiometricImportBatchAsync(CreateBiometricImportBatchDto batch);
    Task<ApiResult<BiometricImportRowDto>> UpsertBiometricImportRowAsync(UpsertBiometricImportRowDto row);
    Task<ApiResult<BiometricImportRowDto>> ReviewBiometricImportRowAsync(ReviewBiometricImportRowDto review);
    Task<ApiResult<BiometricImportBatchDto>> PostBiometricImportBatchAsync(Guid batchId);
    Task<ApiResult<List<PayrollWorkEntryDto>>> GetAttendanceWorkEntriesAsync(Guid companyId, Guid? employeeId = null, DateTime? fromDate = null, DateTime? toDate = null, AttendanceWorkEntryStatus? status = null);
    Task<ApiResult<List<PayrollWorkEntryDto>>> GenerateAttendanceWorkEntriesAsync(GenerateAttendanceWorkEntriesDto request);
    Task<ApiResult<PayrollWorkEntryDto>> UpsertAttendanceWorkEntryAsync(UpsertAttendanceWorkEntryDto entry);
    Task<ApiResult<PayrollWorkEntryDto>> ApproveAttendanceWorkEntryAsync(Guid entryId);
    Task<ApiResult<PayrollWorkEntryDto>> LockAttendanceWorkEntryAsync(Guid entryId);
}

public class LateCheckInReviewResultDto
{
    public LateCheckInRequestDto? Request { get; set; }
    public AttendanceSessionDto? Session { get; set; }
}
