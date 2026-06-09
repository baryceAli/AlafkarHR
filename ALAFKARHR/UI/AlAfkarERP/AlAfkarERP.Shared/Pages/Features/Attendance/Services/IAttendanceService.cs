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
    Task<ApiResult<AttendanceCheckInPreviewDto>> GetCheckInPreviewAsync(Guid employeeId, double? latitude = null, double? longitude = null, double? accuracyMeters = null, DateTime? workDateUtc = null);
    Task<ApiResult<AttendanceCheckInPreviewDto>> GetMyCheckInPreviewAsync(double? latitude = null, double? longitude = null, double? accuracyMeters = null, DateTime? workDateUtc = null);
    Task<ApiResult<PaginatedResult<ShiftAssignmentDto>>> GetShiftAssignmentsAsync(int pageIndex, int pageSize, Guid? companyId = null, ShiftAssignmentScope? scope = null);
    Task<ApiResult<ShiftAssignmentDto>> AssignShiftAsync(AssignShiftDto assignment);
    Task<ApiResult<PaginatedResult<LateCheckInRequestDto>>> GetLateCheckInRequestsAsync(int pageIndex, int pageSize, AttendanceExceptionStatus? status = null, Guid? employeeId = null);
    Task<ApiResult<AttendanceSessionDto>> StartSessionAsync(StartAttendanceSessionDto session);
    Task<ApiResult<AttendanceSessionDto>> EndSessionAsync(Guid sessionId);
    Task<ApiResult<AttendanceSessionDto>> StartBreakAsync(Guid sessionId);
    Task<ApiResult<AttendanceSessionDto>> EndBreakAsync(Guid sessionId);
    Task<ApiResult<bool>> SubmitLocationPingAsync(AttendanceLocationPingDto ping);
    Task<ApiResult<bool>> CreateCheckInAsync(AttendanceCheckInDto checkIn);
    Task<ApiResult<LateCheckInRequestDto>> CreateLateCheckInRequestAsync(CreateLateCheckInRequestDto request);
    Task<ApiResult<LateCheckInReviewResultDto>> ReviewLateCheckInRequestAsync(ReviewLateCheckInRequestDto review);
}

public class LateCheckInReviewResultDto
{
    public LateCheckInRequestDto? Request { get; set; }
    public AttendanceSessionDto? Session { get; set; }
}
