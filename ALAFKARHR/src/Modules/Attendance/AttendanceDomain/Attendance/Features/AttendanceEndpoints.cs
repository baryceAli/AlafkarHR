using AttendanceDomain.Attendance.Features.Breaks;
using AttendanceDomain.Attendance.Features.BreakPolicies;
using AttendanceDomain.Attendance.Features.CheckIns;
using AttendanceDomain.Attendance.Features.EndSession;
using AttendanceDomain.Attendance.Features.Enhancements;
using AttendanceDomain.Attendance.Features.Configuration;
using AttendanceDomain.Attendance.Features.Holidays;
using AttendanceDomain.Attendance.Features.LateCheckInRequests;
using AttendanceDomain.Attendance.Features.LocationPings;
using AttendanceDomain.Attendance.Features.MidDayPermissions;
using AttendanceDomain.Attendance.Features.Queries;
using AttendanceDomain.Attendance.Features.Reports;
using AttendanceDomain.Attendance.Features.SessionNormalization;
using AttendanceDomain.Attendance.Features.ShiftAssignments;
using AttendanceDomain.Attendance.Features.Shifts;
using AttendanceDomain.Attendance.Features.StartSession;
using EmployeeModule.Contracts.Employees.Features.GetEmployeeAttendanceProfile;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Exceptions;
using Shared.Pagination;
using SharedWithUI.Permissions;
using System.Security.Claims;

namespace AttendanceDomain.Attendance.Features;

public record StartAttendanceSessionRequest(StartAttendanceSessionDto Session);
public record EndAttendanceSessionRequest(EndAttendanceSessionDto Session);
public record EndMissingCheckInAttendanceSessionRequest(EndMissingCheckInAttendanceSessionDto Session);
public record NormalizeAttendanceSessionRequest(NormalizeAttendanceSessionDto Session);
public record AttendanceBreakRequest(Guid SessionId);
public record SubmitAttendanceLocationPingRequest(AttendanceLocationPingDto Ping);
public record SubmitAttendanceLocationPingBatchRequest(IReadOnlyCollection<AttendanceLocationPingDto> Pings);
public record CreateAttendanceCheckInRequest(AttendanceCheckInDto CheckIn);
public record CreateLateCheckInRequestRequest(CreateLateCheckInRequestDto Request);
public record ReviewLateCheckInRequestRequest(ReviewLateCheckInRequestDto Review);
public record AssignShiftRequest(AssignShiftDto Assignment);
public record CreateShiftRequest(CreateShiftDto Shift);
public record UpdateShiftRequest(ShiftDto Shift);
public record UpsertAttendanceConfigurationRequest(UpsertAttendanceConfigurationDto Configuration);
public record UpsertAttendanceHolidayRequest(UpsertAttendanceHolidayDto Holiday);
public record UpsertAttendanceBreakPolicyRequest(UpsertAttendanceBreakPolicyDto Policy);
public record CreateMidDayPermissionRequestRequest(CreateMidDayPermissionRequestDto Request);
public record ReviewMidDayPermissionRequestRequest(ReviewMidDayPermissionRequestDto Review);
public record GetAttendanceReportRequest(AttendanceReportFilterDto Filter);
public record UpsertShiftScheduleRequest(UpsertShiftScheduleDto Schedule);
public record UpsertShiftScheduleAssignmentRequest(UpsertShiftScheduleAssignmentDto Assignment);
public record BulkShiftScheduleAssignmentRequest(BulkShiftScheduleAssignmentDto Assignment);
public record CreateShiftSwapRequestBody(CreateShiftSwapRequestDto Request);
public record ReviewShiftSwapRequestBody(ReviewShiftSwapRequestDto Review);
public record CreateAttendanceCorrectionRequestBody(CreateAttendanceCorrectionDto Correction);
public record ReviewAttendanceCorrectionRequestBody(ReviewAttendanceCorrectionDto Review);
public record CreateBiometricImportBatchRequestBody(CreateBiometricImportBatchDto Batch);
public record UpsertBiometricImportRowRequestBody(UpsertBiometricImportRowDto Row);
public record ReviewBiometricImportRowRequestBody(ReviewBiometricImportRowDto Review);
public record GenerateAttendanceWorkEntriesRequestBody(GenerateAttendanceWorkEntriesDto Request);
public record UpsertAttendanceWorkEntryRequestBody(UpsertAttendanceWorkEntryDto Entry);

public class AttendanceEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/attendance")
            .RequireAuthorization()
            .WithTags("Attendance");

        group.MapGet("/dashboard", GetDashboard)
            .WithName("GetAttendanceDashboard")
            .Produces<GetAttendanceDashboardResult>(StatusCodes.Status200OK)
            .WithSummary("Get attendance dashboard")
            .RequireAuthorization(PermissionList.AttendancePermissions.View);

        group.MapGet("/sessions", GetSessions)
            .WithName("GetAttendanceSessions")
            .Produces<GetAttendanceSessionsResult>(StatusCodes.Status200OK)
            .WithSummary("Get attendance sessions")
            .RequireAuthorization(PermissionList.AttendancePermissions.View);

        group.MapGet("/shifts", GetShifts)
            .WithName("GetAttendanceShifts")
            .Produces<GetAttendanceShiftsResult>(StatusCodes.Status200OK)
            .WithSummary("Get attendance shifts")
            .RequireAuthorization(PermissionList.AttendancePermissions.View);

        group.MapPost("/shifts", CreateShift)
            .WithName("CreateAttendanceShift")
            .Produces<CreateShiftResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create an attendance shift")
            .RequireAuthorization(PermissionList.AttendancePermissions.Edit);

        group.MapPut("/shifts/{shiftId:guid}", UpdateShift)
            .WithName("UpdateAttendanceShift")
            .Produces<UpdateShiftResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Update an attendance shift")
            .RequireAuthorization(PermissionList.AttendancePermissions.Edit);

        group.MapDelete("/shifts/{shiftId:guid}", DeleteShift)
            .WithName("DeleteAttendanceShift")
            .Produces<DeleteShiftResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Delete an attendance shift")
            .RequireAuthorization(PermissionList.AttendancePermissions.Edit);

        group.MapGet("/checkin-preview", GetCheckInPreview)
            .WithName("GetAttendanceCheckInPreview")
            .Produces<GetAttendanceCheckInPreviewResult>(StatusCodes.Status200OK)
            .WithSummary("Preview whether the employee can check in now")
            .WithDescription("Loads the effective shift and validates current time and location before starting an attendance session.")
            .RequireAuthorization(PermissionList.AttendancePermissions.Create);

        group.MapGet("/my-checkin-preview", GetMyCheckInPreview)
            .WithName("GetMyAttendanceCheckInPreview")
            .Produces<GetAttendanceCheckInPreviewResult>(StatusCodes.Status200OK)
            .WithSummary("Preview whether the signed-in employee can check in now")
            .WithDescription("Resolves the employee from the signed-in user's employee-id claim or username claim, then validates shift, time, and location.")
            .RequireAuthorization(PermissionList.AttendancePermissions.Create);

        group.MapGet("/late-checkin-requests", GetLateCheckInRequests)
            .WithName("GetLateCheckInRequests")
            .Produces<GetLateCheckInRequestsResult>(StatusCodes.Status200OK)
            .WithSummary("Get late check-in requests")
            .RequireAuthorization(PermissionList.AttendancePermissions.ReviewRequests);

        group.MapGet("/shift-assignments", GetShiftAssignments)
            .WithName("GetShiftAssignments")
            .Produces<GetShiftAssignmentsResult>(StatusCodes.Status200OK)
            .WithSummary("Get shift assignments")
            .RequireAuthorization(PermissionList.AttendancePermissions.View);

        group.MapPost("/sessions/start", StartSession)
            .WithName("StartAttendanceSession")
            .Produces<StartAttendanceSessionResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Start an attendance session")
            .WithDescription("Starts attendance for fixed-location or mobile employees. Fixed-location employees are validated against the assigned department geofence.")
            .RequireAuthorization(PermissionList.AttendancePermissions.Create);

        group.MapPost("/sessions/end", EndSession)
            .WithName("EndAttendanceSession")
            .Produces<EndAttendanceSessionResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("End an attendance session")
            .RequireAuthorization(PermissionList.AttendancePermissions.Create);

        group.MapPost("/sessions/end-missing-checkin", EndMissingCheckInSession)
            .WithName("EndMissingCheckInAttendanceSession")
            .Produces<EndMissingCheckInAttendanceSessionResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Record checkout when the employee missed check-in")
            .RequireAuthorization(PermissionList.AttendancePermissions.Create);

        group.MapPost("/sessions/normalize", NormalizeSession)
            .WithName("NormalizeAttendanceSession")
            .Produces<NormalizeAttendanceSessionResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Normalize missing check-in, missing checkout, or absence")
            .RequireAuthorization(PermissionList.AttendancePermissions.ReviewRequests);

        group.MapPost("/sessions/break/start", StartBreak)
            .WithName("StartAttendanceBreak")
            .Produces<AttendanceBreakResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Pause attendance tracking for a break")
            .RequireAuthorization(PermissionList.AttendancePermissions.Create);

        group.MapPost("/sessions/break/end", EndBreak)
            .WithName("EndAttendanceBreak")
            .Produces<AttendanceBreakResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Resume attendance tracking after a break")
            .RequireAuthorization(PermissionList.AttendancePermissions.Create);

        group.MapPost("/location/ping", SubmitLocationPing)
            .WithName("SubmitAttendanceLocationPing")
            .Produces<SubmitAttendanceLocationPingResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Submit a GPS ping for an active session")
            .RequireAuthorization(PermissionList.AttendancePermissions.Create);

        group.MapPost("/location/ping/batch", SubmitLocationPingBatch)
            .WithName("SubmitAttendanceLocationPingBatch")
            .Produces<SubmitAttendanceLocationPingBatchResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Batch-sync offline GPS pings")
            .RequireAuthorization(PermissionList.AttendancePermissions.Create);

        group.MapPost("/checkins", CreateCheckIn)
            .WithName("CreateAttendanceCheckIn")
            .Produces<CreateAttendanceCheckInResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create a mobile site or task check-in")
            .RequireAuthorization(PermissionList.AttendancePermissions.Create);

        group.MapPost("/late-checkin-requests", CreateLateCheckInRequest)
            .WithName("CreateLateCheckInRequest")
            .Produces<CreateLateCheckInRequestResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Request admin approval for a prohibited late check-in")
            .RequireAuthorization(PermissionList.AttendancePermissions.Create);

        group.MapPost("/late-checkin-requests/review", ReviewLateCheckInRequest)
            .WithName("ReviewLateCheckInRequest")
            .Produces<ReviewLateCheckInRequestResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Approve, adjust, or reject a late check-in request")
            .RequireAuthorization(PermissionList.AttendancePermissions.ReviewRequests);

        group.MapPost("/shift-assignments", AssignShift)
            .WithName("AssignShift")
            .Produces<AssignShiftResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Assign a shift to company, administration, department, or employee")
            .RequireAuthorization(PermissionList.AttendancePermissions.Edit);

        group.MapGet("/configuration", GetConfiguration)
            .WithName("GetAttendanceConfiguration")
            .Produces<GetAttendanceConfigurationResult>(StatusCodes.Status200OK)
            .WithSummary("Get attendance configuration")
            .RequireAuthorization(PermissionList.AttendancePermissions.ViewConfiguration);

        group.MapPut("/configuration", UpsertConfiguration)
            .WithName("UpsertAttendanceConfiguration")
            .Produces<UpsertAttendanceConfigurationResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create or update attendance configuration")
            .RequireAuthorization(PermissionList.AttendancePermissions.ManageConfiguration);

        group.MapGet("/holidays", GetHolidays)
            .WithName("GetAttendanceHolidays")
            .Produces<GetAttendanceHolidaysResult>(StatusCodes.Status200OK)
            .WithSummary("Get predefined attendance holidays")
            .RequireAuthorization(PermissionList.AttendancePermissions.ManageHolidays);

        group.MapPost("/holidays", UpsertHoliday)
            .WithName("UpsertAttendanceHoliday")
            .Produces<UpsertAttendanceHolidayResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create or update a predefined attendance holiday")
            .RequireAuthorization(PermissionList.AttendancePermissions.ManageHolidays);

        group.MapDelete("/holidays/{holidayId:guid}", DeleteHoliday)
            .WithName("DeleteAttendanceHoliday")
            .Produces<DeleteAttendanceHolidayResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Delete a predefined attendance holiday")
            .RequireAuthorization(PermissionList.AttendancePermissions.ManageHolidays);

        group.MapGet("/break-policies", GetBreakPolicies)
            .WithName("GetAttendanceBreakPolicies")
            .Produces<GetAttendanceBreakPoliciesResult>(StatusCodes.Status200OK)
            .WithSummary("Get break policies")
            .RequireAuthorization(PermissionList.AttendancePermissions.ViewConfiguration);

        group.MapPost("/break-policies", UpsertBreakPolicy)
            .WithName("UpsertAttendanceBreakPolicy")
            .Produces<UpsertAttendanceBreakPolicyResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create or update a break policy")
            .RequireAuthorization(PermissionList.AttendancePermissions.ManageConfiguration);

        group.MapGet("/mid-day-permissions", GetMidDayPermissions)
            .WithName("GetMidDayPermissionRequests")
            .Produces<GetMidDayPermissionRequestsResult>(StatusCodes.Status200OK)
            .WithSummary("Get mid-day permission requests")
            .RequireAuthorization(PermissionList.AttendancePermissions.ApproveMidDayPermission);

        group.MapPost("/mid-day-permissions", CreateMidDayPermission)
            .WithName("CreateMidDayPermissionRequest")
            .Produces<CreateMidDayPermissionRequestResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create a mid-day permission request")
            .RequireAuthorization(PermissionList.AttendancePermissions.RequestMidDayPermission);

        group.MapPost("/mid-day-permissions/review", ReviewMidDayPermission)
            .WithName("ReviewMidDayPermissionRequest")
            .Produces<ReviewMidDayPermissionRequestResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Approve, adjust, or reject a mid-day permission request")
            .RequireAuthorization(PermissionList.AttendancePermissions.ApproveMidDayPermission);

        group.MapPost("/reports", GetAttendanceReport)
            .WithName("GetDetailedAttendanceReport")
            .Produces<GetAttendanceReportResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get detailed attendance report rows")
            .RequireAuthorization(PermissionList.AttendancePermissions.ViewReports);

        group.MapGet("/shift-schedules", GetShiftSchedules)
            .WithName("GetShiftSchedules")
            .Produces<GetShiftSchedulesResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.AttendanceRosterPermissions.View);

        group.MapPost("/shift-schedules", UpsertShiftSchedule)
            .WithName("CreateShiftSchedule")
            .Produces<UpsertShiftScheduleResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(PermissionList.AttendanceRosterPermissions.Create);

        group.MapPut("/shift-schedules/{scheduleId:guid}", UpsertShiftSchedule)
            .WithName("UpdateShiftSchedule")
            .Produces<UpsertShiftScheduleResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(PermissionList.AttendanceRosterPermissions.Edit);

        group.MapPost("/shift-schedules/{scheduleId:guid}/publish", PublishShiftSchedule)
            .WithName("PublishShiftSchedule")
            .Produces<UpsertShiftScheduleResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.AttendanceRosterPermissions.Publish);

        group.MapPost("/shift-schedules/{scheduleId:guid}/lock", LockShiftSchedule)
            .WithName("LockShiftSchedule")
            .Produces<UpsertShiftScheduleResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.AttendanceRosterPermissions.Publish);

        group.MapPost("/shift-schedules/{scheduleId:guid}/cancel", CancelShiftSchedule)
            .WithName("CancelShiftSchedule")
            .Produces<UpsertShiftScheduleResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.AttendanceRosterPermissions.Edit);

        group.MapGet("/shift-schedule-assignments", GetShiftScheduleAssignments)
            .WithName("GetShiftScheduleAssignments")
            .Produces<GetShiftScheduleAssignmentsResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.AttendanceRosterPermissions.View);

        group.MapPost("/shift-schedule-assignments", UpsertShiftScheduleAssignment)
            .WithName("CreateShiftScheduleAssignment")
            .Produces<UpsertShiftScheduleAssignmentResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.AttendanceRosterPermissions.Create);

        group.MapPut("/shift-schedule-assignments/{assignmentId:guid}", UpsertShiftScheduleAssignment)
            .WithName("UpdateShiftScheduleAssignment")
            .Produces<UpsertShiftScheduleAssignmentResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.AttendanceRosterPermissions.Edit);

        group.MapPost("/shift-schedule-assignments/bulk", BulkShiftScheduleAssignment)
            .WithName("BulkShiftScheduleAssignment")
            .Produces<BulkShiftScheduleAssignmentResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.AttendanceRosterPermissions.Create);

        group.MapDelete("/shift-schedule-assignments/{assignmentId:guid}", DeleteShiftScheduleAssignment)
            .WithName("DeleteShiftScheduleAssignment")
            .Produces<DeleteShiftScheduleAssignmentResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.AttendanceRosterPermissions.Edit);

        group.MapGet("/shift-swap-requests", GetShiftSwapRequests)
            .WithName("GetShiftSwapRequests")
            .Produces<GetShiftSwapRequestsResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.AttendanceRosterPermissions.View);

        group.MapPost("/shift-swap-requests", CreateShiftSwapRequest)
            .WithName("CreateShiftSwapRequest")
            .Produces<CreateShiftSwapRequestResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.AttendanceRosterPermissions.Create);

        group.MapPost("/shift-swap-requests/review", ReviewShiftSwapRequest)
            .WithName("ReviewShiftSwapRequest")
            .Produces<ReviewShiftSwapRequestResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.AttendanceRosterPermissions.ApproveSwap);

        group.MapPost("/shift-swap-requests/{requestId:guid}/cancel", CancelShiftSwapRequest)
            .WithName("CancelShiftSwapRequest")
            .Produces<ReviewShiftSwapRequestResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.AttendanceRosterPermissions.Edit);

        group.MapGet("/attendance-corrections", GetAttendanceCorrections)
            .WithName("GetAttendanceCorrections")
            .Produces<GetAttendanceCorrectionsResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.AttendanceRosterPermissions.View);

        group.MapPost("/attendance-corrections", CreateAttendanceCorrection)
            .WithName("CreateAttendanceCorrection")
            .Produces<CreateAttendanceCorrectionResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.AttendanceRosterPermissions.Create);

        group.MapPost("/attendance-corrections/review", ReviewAttendanceCorrection)
            .WithName("ReviewAttendanceCorrection")
            .Produces<ReviewAttendanceCorrectionResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.AttendanceRosterPermissions.Edit);

        group.MapPost("/attendance-corrections/{correctionId:guid}/apply", ApplyAttendanceCorrection)
            .WithName("ApplyAttendanceCorrection")
            .Produces<ReviewAttendanceCorrectionResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.AttendanceRosterPermissions.Edit);

        group.MapGet("/device-import-batches", GetBiometricImportBatches)
            .WithName("GetBiometricImportBatches")
            .Produces<GetBiometricImportBatchesResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.AttendanceRosterPermissions.View);

        group.MapPost("/device-import-batches", CreateBiometricImportBatch)
            .WithName("CreateBiometricImportBatch")
            .Produces<CreateBiometricImportBatchResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.AttendanceRosterPermissions.Create);

        group.MapPost("/device-import-rows", UpsertBiometricImportRow)
            .WithName("CreateBiometricImportRow")
            .Produces<UpsertBiometricImportRowResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.AttendanceRosterPermissions.Create);

        group.MapPut("/device-import-rows/{rowId:guid}", UpsertBiometricImportRow)
            .WithName("UpdateBiometricImportRow")
            .Produces<UpsertBiometricImportRowResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.AttendanceRosterPermissions.Edit);

        group.MapPost("/device-import-rows/review", ReviewBiometricImportRow)
            .WithName("ReviewBiometricImportRow")
            .Produces<UpsertBiometricImportRowResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.AttendanceRosterPermissions.Edit);

        group.MapPost("/device-import-batches/{batchId:guid}/post", PostBiometricImportBatch)
            .WithName("PostBiometricImportBatch")
            .Produces<CreateBiometricImportBatchResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.AttendanceRosterPermissions.Publish);

        group.MapGet("/work-entries", GetAttendanceWorkEntries)
            .WithName("GetAttendanceWorkEntries")
            .Produces<GetAttendanceWorkEntriesResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.AttendanceWorkEntryPermissions.View);

        group.MapPost("/work-entries/generate", GenerateAttendanceWorkEntries)
            .WithName("GenerateAttendanceWorkEntries")
            .Produces<GenerateAttendanceWorkEntriesResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.AttendanceWorkEntryPermissions.Generate);

        group.MapPost("/work-entries", UpsertAttendanceWorkEntry)
            .WithName("CreateAttendanceWorkEntry")
            .Produces<UpsertAttendanceWorkEntryResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.AttendanceWorkEntryPermissions.Edit);

        group.MapPut("/work-entries/{entryId:guid}", UpsertAttendanceWorkEntry)
            .WithName("UpdateAttendanceWorkEntry")
            .Produces<UpsertAttendanceWorkEntryResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.AttendanceWorkEntryPermissions.Edit);

        group.MapPost("/work-entries/{entryId:guid}/approve", ApproveAttendanceWorkEntry)
            .WithName("ApproveAttendanceWorkEntry")
            .Produces<UpsertAttendanceWorkEntryResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.AttendanceWorkEntryPermissions.Approve);

        group.MapPost("/work-entries/{entryId:guid}/lock", LockAttendanceWorkEntry)
            .WithName("LockAttendanceWorkEntry")
            .Produces<UpsertAttendanceWorkEntryResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.AttendanceWorkEntryPermissions.Approve);
    }

    private static async Task<Ok<GetAttendanceDashboardResult>> GetDashboard(
        [FromQuery] Guid? employeeId,
        ISender sender)
    {
        var result = await sender.Send(new GetAttendanceDashboardQuery(employeeId));
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<GetAttendanceSessionsResult>> GetSessions(
        [FromQuery] Guid? employeeId,
        [FromQuery] DateTime? fromUtc,
        [FromQuery] DateTime? toUtc,
        [AsParameters] PaginationRequest request,
        ISender sender)
    {
        var result = await sender.Send(new GetAttendanceSessionsQuery(employeeId, fromUtc, toUtc, request));
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<GetAttendanceShiftsResult>> GetShifts(
        [FromQuery] Guid? companyId,
        ISender sender)
    {
        var result = await sender.Send(new GetAttendanceShiftsQuery(companyId));
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<CreateShiftResult>> CreateShift(
        [FromBody] CreateShiftRequest request,
        ISender sender)
    {
        var result = await sender.Send(new CreateShiftCommand(request.Shift));
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<UpdateShiftResult>> UpdateShift(
        Guid shiftId,
        [FromBody] UpdateShiftRequest request,
        ISender sender)
    {
        request.Shift.Id = shiftId;
        var result = await sender.Send(new UpdateShiftCommand(request.Shift));
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<DeleteShiftResult>> DeleteShift(
        Guid shiftId,
        ISender sender)
    {
        var result = await sender.Send(new DeleteShiftCommand(shiftId));
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<GetAttendanceCheckInPreviewResult>> GetCheckInPreview(
        [FromQuery] Guid employeeId,
        [FromQuery] double? latitude,
        [FromQuery] double? longitude,
        [FromQuery] double? accuracyMeters,
        [FromQuery] bool? isMockedLocation,
        [FromQuery] string? locationIntegrityNote,
        [FromQuery] DateTime? workDateUtc,
        ISender sender)
    {
        var result = await sender.Send(new GetAttendanceCheckInPreviewQuery(
            employeeId,
            latitude,
            longitude,
            accuracyMeters,
            isMockedLocation ?? false,
            locationIntegrityNote,
            workDateUtc));

        return TypedResults.Ok(result);
    }

    private static async Task<Ok<GetAttendanceCheckInPreviewResult>> GetMyCheckInPreview(
        [FromQuery] double? latitude,
        [FromQuery] double? longitude,
        [FromQuery] double? accuracyMeters,
        [FromQuery] bool? isMockedLocation,
        [FromQuery] string? locationIntegrityNote,
        [FromQuery] DateTime? workDateUtc,
        ClaimsPrincipal user,
        ISender sender)
    {
        var employeeId = await ResolveSignedInEmployeeIdAsync(user, sender);

        var result = await sender.Send(new GetAttendanceCheckInPreviewQuery(
            employeeId,
            latitude,
            longitude,
            accuracyMeters,
            isMockedLocation ?? false,
            locationIntegrityNote,
            workDateUtc));

        return TypedResults.Ok(result);
    }

    private static async Task<Ok<GetLateCheckInRequestsResult>> GetLateCheckInRequests(
        [FromQuery] AttendanceExceptionStatus? status,
        [FromQuery] Guid? employeeId,
        [AsParameters] PaginationRequest request,
        ISender sender)
    {
        var result = await sender.Send(new GetLateCheckInRequestsQuery(status, employeeId, request));
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<GetShiftAssignmentsResult>> GetShiftAssignments(
        [FromQuery] Guid? companyId,
        [FromQuery] ShiftAssignmentScope? scope,
        [AsParameters] PaginationRequest request,
        ISender sender)
    {
        var result = await sender.Send(new GetShiftAssignmentsQuery(companyId, scope, request));
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<AssignShiftResult>> AssignShift(
        [FromBody] AssignShiftRequest request,
        ISender sender)
    {
        var result = await sender.Send(new AssignShiftCommand(request.Assignment));
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<GetAttendanceConfigurationResult>> GetConfiguration(
        [FromQuery] Guid companyId,
        ISender sender)
    {
        var result = await sender.Send(new GetAttendanceConfigurationQuery(companyId));
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<UpsertAttendanceConfigurationResult>> UpsertConfiguration(
        [FromBody] UpsertAttendanceConfigurationRequest request,
        ClaimsPrincipal user,
        ISender sender)
    {
        var result = await sender.Send(new UpsertAttendanceConfigurationCommand(request.Configuration, user.FindFirstValue(ClaimTypes.NameIdentifier)));
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<GetAttendanceHolidaysResult>> GetHolidays(
        [FromQuery] Guid companyId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        ISender sender)
    {
        var result = await sender.Send(new GetAttendanceHolidaysQuery(companyId, fromDate, toDate));
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<UpsertAttendanceHolidayResult>> UpsertHoliday(
        [FromBody] UpsertAttendanceHolidayRequest request,
        ClaimsPrincipal user,
        ISender sender)
    {
        var result = await sender.Send(new UpsertAttendanceHolidayCommand(request.Holiday, user.FindFirstValue(ClaimTypes.NameIdentifier)));
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<DeleteAttendanceHolidayResult>> DeleteHoliday(
        Guid holidayId,
        ClaimsPrincipal user,
        ISender sender)
    {
        var result = await sender.Send(new DeleteAttendanceHolidayCommand(holidayId, user.FindFirstValue(ClaimTypes.NameIdentifier)));
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<GetAttendanceBreakPoliciesResult>> GetBreakPolicies(
        [FromQuery] Guid companyId,
        ISender sender)
    {
        var result = await sender.Send(new GetAttendanceBreakPoliciesQuery(companyId));
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<UpsertAttendanceBreakPolicyResult>> UpsertBreakPolicy(
        [FromBody] UpsertAttendanceBreakPolicyRequest request,
        ClaimsPrincipal user,
        ISender sender)
    {
        var result = await sender.Send(new UpsertAttendanceBreakPolicyCommand(request.Policy, user.FindFirstValue(ClaimTypes.NameIdentifier)));
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<GetMidDayPermissionRequestsResult>> GetMidDayPermissions(
        [FromQuery] Guid companyId,
        [FromQuery] AttendanceExceptionStatus? status,
        [FromQuery] Guid? employeeId,
        [AsParameters] PaginationRequest request,
        ClaimsPrincipal user,
        ISender sender)
    {
        var reviewerEmployeeId = await ResolveSignedInEmployeeIdAsync(user, sender);

        var result = await sender.Send(new GetMidDayPermissionRequestsQuery(companyId, status, employeeId, reviewerEmployeeId, request));
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<CreateMidDayPermissionRequestResult>> CreateMidDayPermission(
        [FromBody] CreateMidDayPermissionRequestRequest request,
        ClaimsPrincipal user,
        ISender sender)
    {
        var employeeId = await ResolveSignedInEmployeeIdAsync(user, sender);
        var employee = await sender.Send(new GetEmployeeAttendanceProfileQuery(employeeId));
        request.Request.EmployeeId = employee.EmployeeId;
        request.Request.CompanyId = employee.CompanyId;

        var result = await sender.Send(new CreateMidDayPermissionRequestCommand(request.Request));
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<ReviewMidDayPermissionRequestResult>> ReviewMidDayPermission(
        [FromBody] ReviewMidDayPermissionRequestRequest request,
        ClaimsPrincipal user,
        ISender sender)
    {
        var reviewedBy = user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User is not authenticated");
        var reviewerEmployeeId = await ResolveSignedInEmployeeIdAsync(user, sender);

        var result = await sender.Send(new ReviewMidDayPermissionRequestCommand(request.Review, reviewedBy, reviewerEmployeeId));
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<GetAttendanceReportResult>> GetAttendanceReport(
        [FromBody] GetAttendanceReportRequest request,
        ISender sender)
    {
        var result = await sender.Send(new GetAttendanceReportQuery(request.Filter));
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<GetShiftSchedulesResult>> GetShiftSchedules([FromQuery] Guid companyId, [FromQuery] AttendanceRosterStatus? status, ISender sender)
        => TypedResults.Ok(await sender.Send(new GetShiftSchedulesQuery(companyId, status)));

    private static async Task<Ok<UpsertShiftScheduleResult>> UpsertShiftSchedule(Guid? scheduleId, [FromBody] UpsertShiftScheduleRequest request, ClaimsPrincipal user, ISender sender)
    {
        request.Schedule.Id = scheduleId ?? request.Schedule.Id;
        return TypedResults.Ok(await sender.Send(new UpsertShiftScheduleCommand(request.Schedule, user.FindFirstValue(ClaimTypes.NameIdentifier))));
    }

    private static async Task<Ok<UpsertShiftScheduleResult>> PublishShiftSchedule(Guid scheduleId, ClaimsPrincipal user, ISender sender)
        => TypedResults.Ok(await sender.Send(new ChangeShiftScheduleStatusCommand(scheduleId, AttendanceRosterStatus.Published, user.FindFirstValue(ClaimTypes.NameIdentifier))));

    private static async Task<Ok<UpsertShiftScheduleResult>> LockShiftSchedule(Guid scheduleId, ClaimsPrincipal user, ISender sender)
        => TypedResults.Ok(await sender.Send(new ChangeShiftScheduleStatusCommand(scheduleId, AttendanceRosterStatus.Locked, user.FindFirstValue(ClaimTypes.NameIdentifier))));

    private static async Task<Ok<UpsertShiftScheduleResult>> CancelShiftSchedule(Guid scheduleId, ClaimsPrincipal user, ISender sender)
        => TypedResults.Ok(await sender.Send(new ChangeShiftScheduleStatusCommand(scheduleId, AttendanceRosterStatus.Cancelled, user.FindFirstValue(ClaimTypes.NameIdentifier))));

    private static async Task<Ok<GetShiftScheduleAssignmentsResult>> GetShiftScheduleAssignments([FromQuery] Guid? scheduleId, [FromQuery] Guid? employeeId, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate, [AsParameters] PaginationRequest request, ISender sender)
        => TypedResults.Ok(await sender.Send(new GetShiftScheduleAssignmentsQuery(scheduleId, employeeId, fromDate, toDate, request)));

    private static async Task<Ok<UpsertShiftScheduleAssignmentResult>> UpsertShiftScheduleAssignment(Guid? assignmentId, [FromBody] UpsertShiftScheduleAssignmentRequest request, ClaimsPrincipal user, ISender sender)
    {
        request.Assignment.Id = assignmentId ?? request.Assignment.Id;
        return TypedResults.Ok(await sender.Send(new UpsertShiftScheduleAssignmentCommand(request.Assignment, user.FindFirstValue(ClaimTypes.NameIdentifier))));
    }

    private static async Task<Ok<BulkShiftScheduleAssignmentResult>> BulkShiftScheduleAssignment([FromBody] BulkShiftScheduleAssignmentRequest request, ClaimsPrincipal user, ISender sender)
        => TypedResults.Ok(await sender.Send(new BulkShiftScheduleAssignmentCommand(request.Assignment, user.FindFirstValue(ClaimTypes.NameIdentifier))));

    private static async Task<Ok<DeleteShiftScheduleAssignmentResult>> DeleteShiftScheduleAssignment(Guid assignmentId, ClaimsPrincipal user, ISender sender)
        => TypedResults.Ok(await sender.Send(new DeleteShiftScheduleAssignmentCommand(assignmentId, user.FindFirstValue(ClaimTypes.NameIdentifier))));

    private static async Task<Ok<GetShiftSwapRequestsResult>> GetShiftSwapRequests([FromQuery] Guid companyId, [FromQuery] AttendanceExceptionStatus? status, [FromQuery] Guid? employeeId, ISender sender)
        => TypedResults.Ok(await sender.Send(new GetShiftSwapRequestsQuery(companyId, status, employeeId)));

    private static async Task<Ok<CreateShiftSwapRequestResult>> CreateShiftSwapRequest([FromBody] CreateShiftSwapRequestBody request, ISender sender)
        => TypedResults.Ok(await sender.Send(new CreateShiftSwapRequestCommand(request.Request)));

    private static async Task<Ok<ReviewShiftSwapRequestResult>> ReviewShiftSwapRequest([FromBody] ReviewShiftSwapRequestBody request, ClaimsPrincipal user, ISender sender)
        => TypedResults.Ok(await sender.Send(new ReviewShiftSwapRequestCommand(request.Review, user.FindFirstValue(ClaimTypes.NameIdentifier))));

    private static async Task<Ok<ReviewShiftSwapRequestResult>> CancelShiftSwapRequest(Guid requestId, ClaimsPrincipal user, ISender sender)
        => TypedResults.Ok(await sender.Send(new CancelShiftSwapRequestCommand(requestId, user.FindFirstValue(ClaimTypes.NameIdentifier))));

    private static async Task<Ok<GetAttendanceCorrectionsResult>> GetAttendanceCorrections([FromQuery] Guid companyId, [FromQuery] AttendanceExceptionStatus? status, [FromQuery] Guid? employeeId, ISender sender)
        => TypedResults.Ok(await sender.Send(new GetAttendanceCorrectionsQuery(companyId, status, employeeId)));

    private static async Task<Ok<CreateAttendanceCorrectionResult>> CreateAttendanceCorrection([FromBody] CreateAttendanceCorrectionRequestBody request, ISender sender)
        => TypedResults.Ok(await sender.Send(new CreateAttendanceCorrectionCommand(request.Correction)));

    private static async Task<Ok<ReviewAttendanceCorrectionResult>> ReviewAttendanceCorrection([FromBody] ReviewAttendanceCorrectionRequestBody request, ClaimsPrincipal user, ISender sender)
        => TypedResults.Ok(await sender.Send(new ReviewAttendanceCorrectionCommand(request.Review, user.FindFirstValue(ClaimTypes.NameIdentifier))));

    private static async Task<Ok<ReviewAttendanceCorrectionResult>> ApplyAttendanceCorrection(Guid correctionId, ClaimsPrincipal user, ISender sender)
        => TypedResults.Ok(await sender.Send(new ApplyAttendanceCorrectionCommand(correctionId, user.FindFirstValue(ClaimTypes.NameIdentifier))));

    private static async Task<Ok<GetBiometricImportBatchesResult>> GetBiometricImportBatches([FromQuery] Guid companyId, ISender sender)
        => TypedResults.Ok(await sender.Send(new GetBiometricImportBatchesQuery(companyId)));

    private static async Task<Ok<CreateBiometricImportBatchResult>> CreateBiometricImportBatch([FromBody] CreateBiometricImportBatchRequestBody request, ISender sender)
        => TypedResults.Ok(await sender.Send(new CreateBiometricImportBatchCommand(request.Batch)));

    private static async Task<Ok<UpsertBiometricImportRowResult>> UpsertBiometricImportRow(Guid? rowId, [FromBody] UpsertBiometricImportRowRequestBody request, ClaimsPrincipal user, ISender sender)
    {
        request.Row.Id = rowId ?? request.Row.Id;
        return TypedResults.Ok(await sender.Send(new UpsertBiometricImportRowCommand(request.Row, user.FindFirstValue(ClaimTypes.NameIdentifier))));
    }

    private static async Task<Ok<UpsertBiometricImportRowResult>> ReviewBiometricImportRow([FromBody] ReviewBiometricImportRowRequestBody request, ClaimsPrincipal user, ISender sender)
        => TypedResults.Ok(await sender.Send(new ReviewBiometricImportRowCommand(request.Review, user.FindFirstValue(ClaimTypes.NameIdentifier))));

    private static async Task<Ok<CreateBiometricImportBatchResult>> PostBiometricImportBatch(Guid batchId, ClaimsPrincipal user, ISender sender)
        => TypedResults.Ok(await sender.Send(new PostBiometricImportBatchCommand(batchId, user.FindFirstValue(ClaimTypes.NameIdentifier))));

    private static async Task<Ok<GetAttendanceWorkEntriesResult>> GetAttendanceWorkEntries([FromQuery] Guid companyId, [FromQuery] Guid? employeeId, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate, [FromQuery] AttendanceWorkEntryStatus? status, ISender sender)
        => TypedResults.Ok(await sender.Send(new GetAttendanceWorkEntriesQuery(companyId, employeeId, fromDate, toDate, status)));

    private static async Task<Ok<GenerateAttendanceWorkEntriesResult>> GenerateAttendanceWorkEntries([FromBody] GenerateAttendanceWorkEntriesRequestBody request, ClaimsPrincipal user, ISender sender)
        => TypedResults.Ok(await sender.Send(new GenerateAttendanceWorkEntriesCommand(request.Request, user.FindFirstValue(ClaimTypes.NameIdentifier))));

    private static async Task<Ok<UpsertAttendanceWorkEntryResult>> UpsertAttendanceWorkEntry(Guid? entryId, [FromBody] UpsertAttendanceWorkEntryRequestBody request, ClaimsPrincipal user, ISender sender)
    {
        request.Entry.Id = entryId ?? request.Entry.Id;
        return TypedResults.Ok(await sender.Send(new UpsertAttendanceWorkEntryCommand(request.Entry, user.FindFirstValue(ClaimTypes.NameIdentifier))));
    }

    private static async Task<Ok<UpsertAttendanceWorkEntryResult>> ApproveAttendanceWorkEntry(Guid entryId, ClaimsPrincipal user, ISender sender)
        => TypedResults.Ok(await sender.Send(new ChangeAttendanceWorkEntryStatusCommand(entryId, AttendanceWorkEntryStatus.Approved, user.FindFirstValue(ClaimTypes.NameIdentifier))));

    private static async Task<Ok<UpsertAttendanceWorkEntryResult>> LockAttendanceWorkEntry(Guid entryId, ClaimsPrincipal user, ISender sender)
        => TypedResults.Ok(await sender.Send(new ChangeAttendanceWorkEntryStatusCommand(entryId, AttendanceWorkEntryStatus.Locked, user.FindFirstValue(ClaimTypes.NameIdentifier))));

    private static async Task<Ok<StartAttendanceSessionResult>> StartSession(
        [FromBody] StartAttendanceSessionRequest request,
        ISender sender)
    {
        var result = await sender.Send(new StartAttendanceSessionCommand(request.Session));
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<EndAttendanceSessionResult>> EndSession(
        [FromBody] EndAttendanceSessionRequest request,
        ISender sender)
    {
        var result = await sender.Send(new EndAttendanceSessionCommand(request.Session));
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<EndMissingCheckInAttendanceSessionResult>> EndMissingCheckInSession(
        [FromBody] EndMissingCheckInAttendanceSessionRequest request,
        ISender sender)
    {
        var result = await sender.Send(new EndMissingCheckInAttendanceSessionCommand(request.Session));
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<NormalizeAttendanceSessionResult>> NormalizeSession(
        [FromBody] NormalizeAttendanceSessionRequest request,
        ClaimsPrincipal user,
        ISender sender)
    {
        var normalizedBy = user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User is not authenticated");

        var result = await sender.Send(new NormalizeAttendanceSessionCommand(request.Session, normalizedBy));
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<AttendanceBreakResult>> StartBreak(
        [FromBody] AttendanceBreakRequest request,
        ISender sender)
    {
        var result = await sender.Send(new StartAttendanceBreakCommand(request.SessionId));
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<AttendanceBreakResult>> EndBreak(
        [FromBody] AttendanceBreakRequest request,
        ISender sender)
    {
        var result = await sender.Send(new EndAttendanceBreakCommand(request.SessionId));
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<SubmitAttendanceLocationPingResult>> SubmitLocationPing(
        [FromBody] SubmitAttendanceLocationPingRequest request,
        ISender sender)
    {
        var result = await sender.Send(new SubmitAttendanceLocationPingCommand(request.Ping));
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<SubmitAttendanceLocationPingBatchResult>> SubmitLocationPingBatch(
        [FromBody] SubmitAttendanceLocationPingBatchRequest request,
        ISender sender)
    {
        var result = await sender.Send(new SubmitAttendanceLocationPingBatchCommand(request.Pings));
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<CreateAttendanceCheckInResult>> CreateCheckIn(
        [FromBody] CreateAttendanceCheckInRequest request,
        ISender sender)
    {
        var result = await sender.Send(new CreateAttendanceCheckInCommand(request.CheckIn));
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<CreateLateCheckInRequestResult>> CreateLateCheckInRequest(
        [FromBody] CreateLateCheckInRequestRequest request,
        ISender sender)
    {
        var result = await sender.Send(new CreateLateCheckInRequestCommand(request.Request));
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<ReviewLateCheckInRequestResult>> ReviewLateCheckInRequest(
        [FromBody] ReviewLateCheckInRequestRequest request,
        ClaimsPrincipal user,
        ISender sender)
    {
        var reviewedBy = user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User is not authenticated");

        var result = await sender.Send(new ReviewLateCheckInRequestCommand(request.Review, reviewedBy));
        return TypedResults.Ok(result);
    }

    private static Guid? ResolveEmployeeIdClaim(ClaimsPrincipal user)
    {
        var value = FirstClaimValue(
            user,
            "employee_id",
            "employeeId",
            "EmployeeId");

        return Guid.TryParse(value, out var employeeId) ? employeeId : null;
    }

    private static async Task<Guid> ResolveSignedInEmployeeIdAsync(ClaimsPrincipal user, ISender sender)
    {
        var employeeId = ResolveEmployeeIdClaim(user);
        if (employeeId.HasValue)
        {
            return employeeId.Value;
        }

        var userName = FirstClaimValue(
            user,
            ClaimTypes.Name,
            "name",
            "unique_name",
            "preferred_username",
            "upn")
            ?? throw new UnauthorizedAccessException("The signed-in user does not have a username claim that can be matched to an employee code.");

        try
        {
            var employee = await sender.Send(new GetEmployeeAttendanceProfileByCodeQuery(userName));
            return employee.EmployeeId;
        }
        catch (NotFoundException ex)
        {
            throw new UnauthorizedAccessException("This operation requires a linked employee account.", ex);
        }
    }

    private static string? FirstClaimValue(ClaimsPrincipal user, params string[] claimTypes)
    {
        foreach (var claimType in claimTypes)
        {
            var value = user.FindFirstValue(claimType);
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }

        return null;
    }
}
