using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.Attendance.Dtos;
using SharedWithUI.Attendance.Enums;
using System.Globalization;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.Attendance.Services;

public class AttendanceService : BaseApiService, IAttendanceService
{
    private readonly string path;

    public AttendanceService(HttpClient http, ApiConfig apiConfig) : base(http)
    {
        path = $"api/{apiConfig.Version}/attendance";
    }

    public async Task<ApiResult<AttendanceDashboardDto>> GetDashboardAsync(Guid? employeeId = null)
    {
        var url = $"{path}/dashboard";
        if (employeeId.HasValue)
        {
            url += $"?employeeId={employeeId.Value}";
        }

        return await SendAsync<AttendanceDashboardDto>(new HttpRequestMessage(HttpMethod.Get, url), "dashboard");
    }

    public async Task<ApiResult<PaginatedResult<AttendanceSessionDto>>> GetSessionsAsync(
        int pageIndex,
        int pageSize,
        Guid? employeeId = null,
        DateTime? fromUtc = null,
        DateTime? toUtc = null)
    {
        var url = $"{path}/sessions?pageIndex={pageIndex}&pageSize={pageSize}";
        if (employeeId.HasValue) url += $"&employeeId={employeeId.Value}";
        if (fromUtc.HasValue) url += $"&fromUtc={Uri.EscapeDataString(fromUtc.Value.ToUniversalTime().ToString("O"))}";
        if (toUtc.HasValue) url += $"&toUtc={Uri.EscapeDataString(toUtc.Value.ToUniversalTime().ToString("O"))}";

        return await SendAsync<PaginatedResult<AttendanceSessionDto>>(new HttpRequestMessage(HttpMethod.Get, url), "sessionList");
    }

    public async Task<ApiResult<List<ShiftDto>>> GetShiftsAsync(Guid? companyId = null)
    {
        var url = $"{path}/shifts";
        if (companyId.HasValue)
        {
            url += $"?companyId={companyId.Value}";
        }

        return await SendAsync<List<ShiftDto>>(new HttpRequestMessage(HttpMethod.Get, url), "shiftList");
    }

    public async Task<ApiResult<ShiftDto>> CreateShiftAsync(CreateShiftDto shift)
    {
        return await SendAsync<ShiftDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/shifts")
        {
            Content = JsonContent.Create(new { Shift = shift })
        }, "shift");
    }

    public async Task<ApiResult<ShiftDto>> UpdateShiftAsync(ShiftDto shift)
    {
        return await SendAsync<ShiftDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/shifts/{shift.Id}")
        {
            Content = JsonContent.Create(new { Shift = shift })
        }, "shift");
    }

    public async Task<ApiResult<bool>> DeleteShiftAsync(Guid shiftId)
    {
        return await SendAsync<bool>(new HttpRequestMessage(HttpMethod.Delete, $"{path}/shifts/{shiftId}"), "isSuccess");
    }

    public async Task<ApiResult<AttendanceCheckInPreviewDto>> GetCheckInPreviewAsync(
        Guid employeeId,
        double? latitude = null,
        double? longitude = null,
        double? accuracyMeters = null,
        DateTime? workDateUtc = null)
    {
        var url = $"{path}/checkin-preview?employeeId={employeeId}";
        if (latitude.HasValue) url += $"&latitude={latitude.Value}";
        if (longitude.HasValue) url += $"&longitude={longitude.Value}";
        if (accuracyMeters.HasValue) url += $"&accuracyMeters={accuracyMeters.Value}";
        if (workDateUtc.HasValue) url += $"&workDateUtc={Uri.EscapeDataString(workDateUtc.Value.ToUniversalTime().ToString("O"))}";

        return await SendAsync<AttendanceCheckInPreviewDto>(new HttpRequestMessage(HttpMethod.Get, url), "preview");
    }

    public async Task<ApiResult<AttendanceCheckInPreviewDto>> GetMyCheckInPreviewAsync(
        double? latitude = null,
        double? longitude = null,
        double? accuracyMeters = null,
        DateTime? workDateUtc = null)
    {
        var url = $"{path}/my-checkin-preview";
        var separator = "?";

        void AddQuery(string name, string value)
        {
            url += $"{separator}{name}={Uri.EscapeDataString(value)}";
            separator = "&";
        }

        if (latitude.HasValue) AddQuery("latitude", latitude.Value.ToString(CultureInfo.InvariantCulture));
        if (longitude.HasValue) AddQuery("longitude", longitude.Value.ToString(CultureInfo.InvariantCulture));
        if (accuracyMeters.HasValue) AddQuery("accuracyMeters", accuracyMeters.Value.ToString(CultureInfo.InvariantCulture));
        if (workDateUtc.HasValue) AddQuery("workDateUtc", workDateUtc.Value.ToUniversalTime().ToString("O"));

        return await SendAsync<AttendanceCheckInPreviewDto>(new HttpRequestMessage(HttpMethod.Get, url), "preview");
    }

    public async Task<ApiResult<PaginatedResult<ShiftAssignmentDto>>> GetShiftAssignmentsAsync(
        int pageIndex,
        int pageSize,
        Guid? companyId = null,
        ShiftAssignmentScope? scope = null)
    {
        var url = $"{path}/shift-assignments?pageIndex={pageIndex}&pageSize={pageSize}";
        if (companyId.HasValue) url += $"&companyId={companyId.Value}";
        if (scope.HasValue) url += $"&scope={scope.Value}";

        return await SendAsync<PaginatedResult<ShiftAssignmentDto>>(new HttpRequestMessage(HttpMethod.Get, url), "assignmentList");
    }

    public async Task<ApiResult<ShiftAssignmentDto>> AssignShiftAsync(AssignShiftDto assignment)
    {
        return await SendAsync<ShiftAssignmentDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/shift-assignments")
        {
            Content = JsonContent.Create(new { Assignment = assignment })
        }, "assignment");
    }

    public async Task<ApiResult<PaginatedResult<LateCheckInRequestDto>>> GetLateCheckInRequestsAsync(
        int pageIndex,
        int pageSize,
        AttendanceExceptionStatus? status = null,
        Guid? employeeId = null)
    {
        var url = $"{path}/late-checkin-requests?pageIndex={pageIndex}&pageSize={pageSize}";
        if (status.HasValue) url += $"&status={status.Value}";
        if (employeeId.HasValue) url += $"&employeeId={employeeId.Value}";

        return await SendAsync<PaginatedResult<LateCheckInRequestDto>>(new HttpRequestMessage(HttpMethod.Get, url), "requestList");
    }

    public async Task<ApiResult<AttendanceSessionDto>> StartSessionAsync(StartAttendanceSessionDto session)
    {
        return await SendAsync<AttendanceSessionDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/sessions/start")
        {
            Content = JsonContent.Create(new { Session = session })
        }, "session");
    }

    public async Task<ApiResult<AttendanceSessionDto>> EndSessionAsync(Guid sessionId)
    {
        return await SendAsync<AttendanceSessionDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/sessions/end")
        {
            Content = JsonContent.Create(new { Session = new EndAttendanceSessionDto { SessionId = sessionId } })
        }, "session");
    }

    public async Task<ApiResult<AttendanceSessionDto>> StartBreakAsync(Guid sessionId)
    {
        return await SendAsync<AttendanceSessionDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/sessions/break/start")
        {
            Content = JsonContent.Create(new { SessionId = sessionId })
        }, "session");
    }

    public async Task<ApiResult<AttendanceSessionDto>> EndBreakAsync(Guid sessionId)
    {
        return await SendAsync<AttendanceSessionDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/sessions/break/end")
        {
            Content = JsonContent.Create(new { SessionId = sessionId })
        }, "session");
    }

    public async Task<ApiResult<bool>> SubmitLocationPingAsync(AttendanceLocationPingDto ping)
    {
        return await SendAsync<bool>(new HttpRequestMessage(HttpMethod.Post, $"{path}/location/ping")
        {
            Content = JsonContent.Create(new { Ping = ping })
        }, "isIdle");
    }

    public async Task<ApiResult<bool>> CreateCheckInAsync(AttendanceCheckInDto checkIn)
    {
        return await SendAsync<bool>(new HttpRequestMessage(HttpMethod.Post, $"{path}/checkins")
        {
            Content = JsonContent.Create(new { CheckIn = checkIn })
        }, "isSuccess");
    }

    public async Task<ApiResult<LateCheckInRequestDto>> CreateLateCheckInRequestAsync(CreateLateCheckInRequestDto request)
    {
        return await SendAsync<LateCheckInRequestDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/late-checkin-requests")
        {
            Content = JsonContent.Create(new { Request = request })
        }, "request");
    }

    public async Task<ApiResult<LateCheckInReviewResultDto>> ReviewLateCheckInRequestAsync(ReviewLateCheckInRequestDto review)
    {
        return await SendAsync<LateCheckInReviewResultDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/late-checkin-requests/review")
        {
            Content = JsonContent.Create(new { Review = review })
        }, null);
    }
}
