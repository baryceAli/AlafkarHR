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
    private readonly string leavePath;

    public AttendanceService(HttpClient http, ITokenService tokenService, ApiConfig apiConfig) : base(http, tokenService, apiConfig)
    {
        path = $"api/{apiConfig.Version}/attendance";
        leavePath = $"api/{apiConfig.Version}/leave";
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
        bool isMockedLocation = false,
        string? locationIntegrityNote = null,
        DateTime? workDateUtc = null)
    {
        var url = $"{path}/checkin-preview?employeeId={employeeId}";
        if (latitude.HasValue) url += $"&latitude={latitude.Value.ToString(CultureInfo.InvariantCulture)}";
        if (longitude.HasValue) url += $"&longitude={longitude.Value.ToString(CultureInfo.InvariantCulture)}";
        if (accuracyMeters.HasValue) url += $"&accuracyMeters={accuracyMeters.Value.ToString(CultureInfo.InvariantCulture)}";
        if (isMockedLocation) url += $"&isMockedLocation=true";
        if (!string.IsNullOrWhiteSpace(locationIntegrityNote)) url += $"&locationIntegrityNote={Uri.EscapeDataString(locationIntegrityNote)}";
        if (workDateUtc.HasValue) url += $"&workDateUtc={Uri.EscapeDataString(workDateUtc.Value.ToUniversalTime().ToString("O"))}";

        return await SendAsync<AttendanceCheckInPreviewDto>(new HttpRequestMessage(HttpMethod.Get, url), "preview");
    }

    public async Task<ApiResult<AttendanceCheckInPreviewDto>> GetMyCheckInPreviewAsync(
        double? latitude = null,
        double? longitude = null,
        double? accuracyMeters = null,
        bool isMockedLocation = false,
        string? locationIntegrityNote = null,
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
        if (isMockedLocation) AddQuery("isMockedLocation", "true");
        if (!string.IsNullOrWhiteSpace(locationIntegrityNote)) AddQuery("locationIntegrityNote", locationIntegrityNote);
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

    public async Task<ApiResult<AttendanceSessionDto>> EndMissingCheckInSessionAsync(EndMissingCheckInAttendanceSessionDto session)
    {
        return await SendAsync<AttendanceSessionDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/sessions/end-missing-checkin")
        {
            Content = JsonContent.Create(new { Session = session })
        }, "session");
    }

    public async Task<ApiResult<AttendanceSessionDto>> NormalizeSessionAsync(NormalizeAttendanceSessionDto session)
    {
        return await SendAsync<AttendanceSessionDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/sessions/normalize")
        {
            Content = JsonContent.Create(new { Session = session })
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

    public async Task<ApiResult<AttendanceCalendarSettingsDto>> GetCalendarSettingsAsync(Guid companyId)
    {
        return await SendAsync<AttendanceCalendarSettingsDto>(
            new HttpRequestMessage(HttpMethod.Get, $"{path}/calendar-settings?companyId={companyId}"),
            "settings");
    }

    public async Task<ApiResult<AttendanceCalendarSettingsDto>> UpsertCalendarSettingsAsync(UpsertAttendanceCalendarSettingsDto settings)
    {
        return await SendAsync<AttendanceCalendarSettingsDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/calendar-settings")
        {
            Content = JsonContent.Create(new { Settings = settings })
        }, "settings");
    }

    public async Task<ApiResult<List<AttendanceHolidayDto>>> GetHolidaysAsync(Guid companyId, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var url = $"{path}/holidays?companyId={companyId}";
        if (fromDate.HasValue) url += $"&fromDate={Uri.EscapeDataString(fromDate.Value.ToUniversalTime().ToString("O"))}";
        if (toDate.HasValue) url += $"&toDate={Uri.EscapeDataString(toDate.Value.ToUniversalTime().ToString("O"))}";

        return await SendAsync<List<AttendanceHolidayDto>>(new HttpRequestMessage(HttpMethod.Get, url), "holidayList");
    }

    public async Task<ApiResult<AttendanceHolidayDto>> UpsertHolidayAsync(UpsertAttendanceHolidayDto holiday)
    {
        return await SendAsync<AttendanceHolidayDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/holidays")
        {
            Content = JsonContent.Create(new { Holiday = holiday })
        }, "holiday");
    }

    public async Task<ApiResult<bool>> DeleteHolidayAsync(Guid holidayId)
    {
        return await SendAsync<bool>(new HttpRequestMessage(HttpMethod.Delete, $"{path}/holidays/{holidayId}"), "isSuccess");
    }

    public async Task<ApiResult<PaginatedResult<MidDayPermissionRequestDto>>> GetMidDayPermissionsAsync(
        Guid companyId,
        int pageIndex,
        int pageSize,
        AttendanceExceptionStatus? status = null,
        Guid? employeeId = null)
    {
        var url = $"{path}/mid-day-permissions?companyId={companyId}&pageIndex={pageIndex}&pageSize={pageSize}";
        if (status.HasValue) url += $"&status={status.Value}";
        if (employeeId.HasValue) url += $"&employeeId={employeeId.Value}";

        return await SendAsync<PaginatedResult<MidDayPermissionRequestDto>>(new HttpRequestMessage(HttpMethod.Get, url), "requestList");
    }

    public async Task<ApiResult<MidDayPermissionRequestDto>> CreateMidDayPermissionAsync(CreateMidDayPermissionRequestDto request)
    {
        return await SendAsync<MidDayPermissionRequestDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/mid-day-permissions")
        {
            Content = JsonContent.Create(new { Request = request })
        }, "request");
    }

    public async Task<ApiResult<MidDayPermissionRequestDto>> ReviewMidDayPermissionAsync(ReviewMidDayPermissionRequestDto review)
    {
        return await SendAsync<MidDayPermissionRequestDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/mid-day-permissions/review")
        {
            Content = JsonContent.Create(new { Review = review })
        }, "request");
    }

    public async Task<ApiResult<AttendanceReportDto>> GetReportAsync(AttendanceReportFilterDto filter)
    {
        return await SendAsync<AttendanceReportDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/reports")
        {
            Content = JsonContent.Create(new { Filter = filter })
        }, "report");
    }

    public async Task<ApiResult<AttendanceRosterControlDto>> GetRosterControlAsync(AttendanceRosterControlFilterDto filter)
    {
        var url = $"{leavePath}/attendance-roster-control?companyId={filter.CompanyId}"
            + $"&fromDate={Uri.EscapeDataString(filter.FromDate.ToUniversalTime().ToString("O"))}"
            + $"&toDate={Uri.EscapeDataString(filter.ToDate.ToUniversalTime().ToString("O"))}";

        if (filter.DepartmentId.HasValue) url += $"&departmentId={filter.DepartmentId.Value}";
        if (filter.ShiftId.HasValue) url += $"&shiftId={filter.ShiftId.Value}";
        if (filter.Status.HasValue) url += $"&status={filter.Status.Value}";

        return await SendAsync<AttendanceRosterControlDto>(new HttpRequestMessage(HttpMethod.Get, url), "roster");
    }

    public async Task<ApiResult<List<AttendanceRosterSubstituteConfigurationDto>>> GetRosterSubstituteConfigurationsAsync(Guid companyId)
        => await SendAsync<List<AttendanceRosterSubstituteConfigurationDto>>(
            new HttpRequestMessage(HttpMethod.Get, $"{path}/roster-substitute-configurations?companyId={companyId}"),
            "configurationList");

    public async Task<ApiResult<AttendanceRosterSubstituteConfigurationDto>> UpsertRosterSubstituteConfigurationAsync(
        UpsertAttendanceRosterSubstituteConfigurationDto configuration)
    {
        var method = configuration.Id.HasValue && configuration.Id.Value != Guid.Empty ? HttpMethod.Put : HttpMethod.Post;
        var url = configuration.Id.HasValue && configuration.Id.Value != Guid.Empty
            ? $"{path}/roster-substitute-configurations/{configuration.Id.Value}"
            : $"{path}/roster-substitute-configurations";

        return await SendAsync<AttendanceRosterSubstituteConfigurationDto>(new HttpRequestMessage(method, url)
        {
            Content = JsonContent.Create(new { Configuration = configuration })
        }, "configuration");
    }

    public async Task<ApiResult<List<ShiftScheduleDto>>> GetShiftSchedulesAsync(Guid companyId, AttendanceRosterStatus? status = null)
    {
        var url = $"{path}/shift-schedules?companyId={companyId}";
        if (status.HasValue) url += $"&status={status.Value}";
        return await SendAsync<List<ShiftScheduleDto>>(new HttpRequestMessage(HttpMethod.Get, url), "scheduleList");
    }

    public async Task<ApiResult<ShiftScheduleDto>> UpsertShiftScheduleAsync(UpsertShiftScheduleDto schedule)
    {
        var method = schedule.Id.HasValue && schedule.Id.Value != Guid.Empty ? HttpMethod.Put : HttpMethod.Post;
        var url = schedule.Id.HasValue && schedule.Id.Value != Guid.Empty ? $"{path}/shift-schedules/{schedule.Id.Value}" : $"{path}/shift-schedules";
        return await SendAsync<ShiftScheduleDto>(new HttpRequestMessage(method, url)
        {
            Content = JsonContent.Create(new { Schedule = schedule })
        }, "schedule");
    }

    public async Task<ApiResult<ShiftScheduleDto>> PublishShiftScheduleAsync(Guid scheduleId)
        => await SendAsync<ShiftScheduleDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/shift-schedules/{scheduleId}/publish"), "schedule");

    public async Task<ApiResult<ShiftScheduleDto>> LockShiftScheduleAsync(Guid scheduleId)
        => await SendAsync<ShiftScheduleDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/shift-schedules/{scheduleId}/lock"), "schedule");

    public async Task<ApiResult<ShiftScheduleDto>> CancelShiftScheduleAsync(Guid scheduleId)
        => await SendAsync<ShiftScheduleDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/shift-schedules/{scheduleId}/cancel"), "schedule");

    public async Task<ApiResult<PaginatedResult<ShiftScheduleAssignmentDto>>> GetShiftScheduleAssignmentsAsync(int pageIndex, int pageSize, Guid? scheduleId = null, Guid? employeeId = null, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var url = $"{path}/shift-schedule-assignments?pageIndex={pageIndex}&pageSize={pageSize}";
        if (scheduleId.HasValue) url += $"&scheduleId={scheduleId.Value}";
        if (employeeId.HasValue) url += $"&employeeId={employeeId.Value}";
        if (fromDate.HasValue) url += $"&fromDate={Uri.EscapeDataString(fromDate.Value.ToUniversalTime().ToString("O"))}";
        if (toDate.HasValue) url += $"&toDate={Uri.EscapeDataString(toDate.Value.ToUniversalTime().ToString("O"))}";
        return await SendAsync<PaginatedResult<ShiftScheduleAssignmentDto>>(new HttpRequestMessage(HttpMethod.Get, url), "assignmentList");
    }

    public async Task<ApiResult<ShiftScheduleAssignmentDto>> UpsertShiftScheduleAssignmentAsync(UpsertShiftScheduleAssignmentDto assignment)
    {
        var method = assignment.Id.HasValue && assignment.Id.Value != Guid.Empty ? HttpMethod.Put : HttpMethod.Post;
        var url = assignment.Id.HasValue && assignment.Id.Value != Guid.Empty ? $"{path}/shift-schedule-assignments/{assignment.Id.Value}" : $"{path}/shift-schedule-assignments";
        return await SendAsync<ShiftScheduleAssignmentDto>(new HttpRequestMessage(method, url)
        {
            Content = JsonContent.Create(new { Assignment = assignment })
        }, "assignment");
    }

    public async Task<ApiResult<int>> BulkShiftScheduleAssignmentAsync(BulkShiftScheduleAssignmentDto assignment)
    {
        return await SendAsync<int>(new HttpRequestMessage(HttpMethod.Post, $"{path}/shift-schedule-assignments/bulk")
        {
            Content = JsonContent.Create(new { Assignment = assignment })
        }, "createdCount");
    }

    public async Task<ApiResult<bool>> DeleteShiftScheduleAssignmentAsync(Guid assignmentId)
        => await SendAsync<bool>(new HttpRequestMessage(HttpMethod.Delete, $"{path}/shift-schedule-assignments/{assignmentId}"), "isSuccess");

    public async Task<ApiResult<List<ShiftSwapRequestDto>>> GetShiftSwapRequestsAsync(Guid companyId, AttendanceExceptionStatus? status = null, Guid? employeeId = null)
    {
        var url = $"{path}/shift-swap-requests?companyId={companyId}";
        if (status.HasValue) url += $"&status={status.Value}";
        if (employeeId.HasValue) url += $"&employeeId={employeeId.Value}";
        return await SendAsync<List<ShiftSwapRequestDto>>(new HttpRequestMessage(HttpMethod.Get, url), "requestList");
    }

    public async Task<ApiResult<ShiftSwapRequestDto>> CreateShiftSwapRequestAsync(CreateShiftSwapRequestDto request)
    {
        return await SendAsync<ShiftSwapRequestDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/shift-swap-requests")
        {
            Content = JsonContent.Create(new { Request = request })
        }, "request");
    }

    public async Task<ApiResult<ShiftSwapRequestDto>> ReviewShiftSwapRequestAsync(ReviewShiftSwapRequestDto review)
    {
        return await SendAsync<ShiftSwapRequestDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/shift-swap-requests/review")
        {
            Content = JsonContent.Create(new { Review = review })
        }, "request");
    }

    public async Task<ApiResult<ShiftSwapRequestDto>> CancelShiftSwapRequestAsync(Guid requestId)
        => await SendAsync<ShiftSwapRequestDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/shift-swap-requests/{requestId}/cancel"), "request");

    public async Task<ApiResult<List<AttendanceCorrectionDto>>> GetAttendanceCorrectionsAsync(Guid companyId, AttendanceExceptionStatus? status = null, Guid? employeeId = null)
    {
        var url = $"{path}/attendance-corrections?companyId={companyId}";
        if (status.HasValue) url += $"&status={status.Value}";
        if (employeeId.HasValue) url += $"&employeeId={employeeId.Value}";
        return await SendAsync<List<AttendanceCorrectionDto>>(new HttpRequestMessage(HttpMethod.Get, url), "correctionList");
    }

    public async Task<ApiResult<AttendanceCorrectionDto>> CreateAttendanceCorrectionAsync(CreateAttendanceCorrectionDto correction)
    {
        return await SendAsync<AttendanceCorrectionDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/attendance-corrections")
        {
            Content = JsonContent.Create(new { Correction = correction })
        }, "correction");
    }

    public async Task<ApiResult<AttendanceCorrectionDto>> ReviewAttendanceCorrectionAsync(ReviewAttendanceCorrectionDto review)
    {
        return await SendAsync<AttendanceCorrectionDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/attendance-corrections/review")
        {
            Content = JsonContent.Create(new { Review = review })
        }, "correction");
    }

    public async Task<ApiResult<AttendanceCorrectionDto>> ApplyAttendanceCorrectionAsync(Guid correctionId)
        => await SendAsync<AttendanceCorrectionDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/attendance-corrections/{correctionId}/apply"), "correction");

    public async Task<ApiResult<List<BiometricImportBatchDto>>> GetBiometricImportBatchesAsync(Guid companyId)
        => await SendAsync<List<BiometricImportBatchDto>>(new HttpRequestMessage(HttpMethod.Get, $"{path}/device-import-batches?companyId={companyId}"), "batchList");

    public async Task<ApiResult<BiometricImportBatchDto>> CreateBiometricImportBatchAsync(CreateBiometricImportBatchDto batch)
    {
        return await SendAsync<BiometricImportBatchDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/device-import-batches")
        {
            Content = JsonContent.Create(new { Batch = batch })
        }, "batch");
    }

    public async Task<ApiResult<BiometricImportRowDto>> UpsertBiometricImportRowAsync(UpsertBiometricImportRowDto row)
    {
        var method = row.Id.HasValue && row.Id.Value != Guid.Empty ? HttpMethod.Put : HttpMethod.Post;
        var url = row.Id.HasValue && row.Id.Value != Guid.Empty ? $"{path}/device-import-rows/{row.Id.Value}" : $"{path}/device-import-rows";
        return await SendAsync<BiometricImportRowDto>(new HttpRequestMessage(method, url)
        {
            Content = JsonContent.Create(new { Row = row })
        }, "row");
    }

    public async Task<ApiResult<BiometricImportRowDto>> ReviewBiometricImportRowAsync(ReviewBiometricImportRowDto review)
    {
        return await SendAsync<BiometricImportRowDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/device-import-rows/review")
        {
            Content = JsonContent.Create(new { Review = review })
        }, "row");
    }

    public async Task<ApiResult<BiometricImportBatchDto>> PostBiometricImportBatchAsync(Guid batchId)
        => await SendAsync<BiometricImportBatchDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/device-import-batches/{batchId}/post"), "batch");

    public async Task<ApiResult<List<PayrollWorkEntryDto>>> GetAttendanceWorkEntriesAsync(Guid companyId, Guid? employeeId = null, DateTime? fromDate = null, DateTime? toDate = null, AttendanceWorkEntryStatus? status = null)
    {
        var url = $"{path}/work-entries?companyId={companyId}";
        if (employeeId.HasValue) url += $"&employeeId={employeeId.Value}";
        if (fromDate.HasValue) url += $"&fromDate={Uri.EscapeDataString(fromDate.Value.ToUniversalTime().ToString("O"))}";
        if (toDate.HasValue) url += $"&toDate={Uri.EscapeDataString(toDate.Value.ToUniversalTime().ToString("O"))}";
        if (status.HasValue) url += $"&status={status.Value}";
        return await SendAsync<List<PayrollWorkEntryDto>>(new HttpRequestMessage(HttpMethod.Get, url), "entryList");
    }

    public async Task<ApiResult<List<PayrollWorkEntryDto>>> GenerateAttendanceWorkEntriesAsync(GenerateAttendanceWorkEntriesDto request)
    {
        return await SendAsync<List<PayrollWorkEntryDto>>(new HttpRequestMessage(HttpMethod.Post, $"{path}/work-entries/generate")
        {
            Content = JsonContent.Create(new { Request = request })
        }, "entryList");
    }

    public async Task<ApiResult<PayrollWorkEntryDto>> UpsertAttendanceWorkEntryAsync(UpsertAttendanceWorkEntryDto entry)
    {
        var method = entry.Id.HasValue && entry.Id.Value != Guid.Empty ? HttpMethod.Put : HttpMethod.Post;
        var url = entry.Id.HasValue && entry.Id.Value != Guid.Empty ? $"{path}/work-entries/{entry.Id.Value}" : $"{path}/work-entries";
        return await SendAsync<PayrollWorkEntryDto>(new HttpRequestMessage(method, url)
        {
            Content = JsonContent.Create(new { Entry = entry })
        }, "entry");
    }

    public async Task<ApiResult<PayrollWorkEntryDto>> ApproveAttendanceWorkEntryAsync(Guid entryId)
        => await SendAsync<PayrollWorkEntryDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/work-entries/{entryId}/approve"), "entry");

    public async Task<ApiResult<PayrollWorkEntryDto>> LockAttendanceWorkEntryAsync(Guid entryId)
        => await SendAsync<PayrollWorkEntryDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/work-entries/{entryId}/lock"), "entry");
}
