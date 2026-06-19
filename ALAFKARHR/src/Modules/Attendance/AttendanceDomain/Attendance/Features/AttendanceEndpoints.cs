using AttendanceDomain.Attendance.Features.Breaks;
using AttendanceDomain.Attendance.Features.BreakPolicies;
using AttendanceDomain.Attendance.Features.CheckIns;
using AttendanceDomain.Attendance.Features.EndSession;
using AttendanceDomain.Attendance.Features.Configuration;
using AttendanceDomain.Attendance.Features.EmergencyLeaves;
using AttendanceDomain.Attendance.Features.Holidays;
using AttendanceDomain.Attendance.Features.LateCheckInRequests;
using AttendanceDomain.Attendance.Features.LeaveBalances;
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
public record CreateEmergencyLeaveRequestRequest(CreateEmergencyLeaveRequestDto Request);
public record ReviewEmergencyLeaveRequestRequest(ReviewEmergencyLeaveRequestDto Review);
public record UpsertEmployeeLeaveBalanceRequest(UpsertEmployeeLeaveBalanceDto Balance);
public record GetLeaveReportRequest(LeaveReportFilterDto Filter);
public record CreateMidDayPermissionRequestRequest(CreateMidDayPermissionRequestDto Request);
public record ReviewMidDayPermissionRequestRequest(ReviewMidDayPermissionRequestDto Review);
public record GetAttendanceReportRequest(AttendanceReportFilterDto Filter);

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

        group.MapGet("/emergency-leaves", GetEmergencyLeaves)
            .WithName("GetEmergencyLeaveRequests")
            .Produces<GetEmergencyLeaveRequestsResult>(StatusCodes.Status200OK)
            .WithSummary("Get emergency leave requests")
            .RequireAuthorization(PermissionList.AttendancePermissions.ApproveEmergencyLeave);

        group.MapPost("/emergency-leaves", CreateEmergencyLeave)
            .WithName("CreateEmergencyLeaveRequest")
            .Produces<CreateEmergencyLeaveRequestResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create an emergency leave request")
            .RequireAuthorization(PermissionList.AttendancePermissions.RequestEmergencyLeave);

        group.MapPost("/emergency-leaves/attachments", UploadEmergencyLeaveAttachment)
            .WithName("UploadEmergencyLeaveAttachment")
            .Accepts<IFormFile>("multipart/form-data")
            .Produces<UploadEmergencyLeaveAttachmentResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Upload an emergency leave attachment")
            .RequireAuthorization(PermissionList.AttendancePermissions.RequestEmergencyLeave)
            .DisableAntiforgery();

        group.MapPost("/emergency-leaves/review", ReviewEmergencyLeave)
            .WithName("ReviewEmergencyLeaveRequest")
            .Produces<ReviewEmergencyLeaveRequestResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Approve or reject an emergency leave request")
            .RequireAuthorization(PermissionList.AttendancePermissions.ApproveEmergencyLeave);

        group.MapGet("/leave-balances", GetEmployeeLeaveBalances)
            .WithName("GetEmployeeLeaveBalances")
            .Produces<GetEmployeeLeaveBalancesResult>(StatusCodes.Status200OK)
            .WithSummary("Get employee leave balances")
            .RequireAuthorization(PermissionList.AttendancePermissions.ViewLeaveBalances);

        group.MapPost("/leave-balances", UpsertEmployeeLeaveBalance)
            .WithName("UpsertEmployeeLeaveBalance")
            .Produces<UpsertEmployeeLeaveBalanceResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create or update employee yearly leave balance")
            .RequireAuthorization(PermissionList.AttendancePermissions.ManageLeaveBalances);

        group.MapPost("/leave-reports", GetLeaveReport)
            .WithName("GetLeaveReport")
            .Produces<GetLeaveReportResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get leave balance and usage report")
            .RequireAuthorization(PermissionList.AttendancePermissions.ViewLeaveReports);

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

    private static async Task<Ok<GetEmergencyLeaveRequestsResult>> GetEmergencyLeaves(
        [FromQuery] Guid companyId,
        [FromQuery] AttendanceExceptionStatus? status,
        [FromQuery] Guid? employeeId,
        [AsParameters] PaginationRequest request,
        ClaimsPrincipal user,
        ISender sender)
    {
        var reviewerEmployeeId = await ResolveSignedInEmployeeIdAsync(user, sender);

        var result = await sender.Send(new GetEmergencyLeaveRequestsQuery(companyId, status, employeeId, reviewerEmployeeId, request));
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<CreateEmergencyLeaveRequestResult>> CreateEmergencyLeave(
        [FromBody] CreateEmergencyLeaveRequestRequest request,
        ClaimsPrincipal user,
        ISender sender)
    {
        var employeeId = await ResolveSignedInEmployeeIdAsync(user, sender);

        var employee = await sender.Send(new GetEmployeeAttendanceProfileQuery(employeeId));
        request.Request.EmployeeId = employee.EmployeeId;
        request.Request.CompanyId = employee.CompanyId;

        var result = await sender.Send(new CreateEmergencyLeaveRequestCommand(request.Request));
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<UploadEmergencyLeaveAttachmentResult>> UploadEmergencyLeaveAttachment(
        IFormFile file,
        ClaimsPrincipal user,
        ISender sender)
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User is not authenticated");

        var result = await sender.Send(new UploadEmergencyLeaveAttachmentCommand(file, userId));
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<ReviewEmergencyLeaveRequestResult>> ReviewEmergencyLeave(
        [FromBody] ReviewEmergencyLeaveRequestRequest request,
        ClaimsPrincipal user,
        ISender sender)
    {
        var reviewedBy = user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User is not authenticated");
        var reviewerEmployeeId = await ResolveSignedInEmployeeIdAsync(user, sender);

        var result = await sender.Send(new ReviewEmergencyLeaveRequestCommand(request.Review, reviewedBy, reviewerEmployeeId));
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<GetEmployeeLeaveBalancesResult>> GetEmployeeLeaveBalances(
        [FromQuery] Guid companyId,
        [FromQuery] int year,
        [FromQuery] Guid? employeeId,
        ISender sender)
    {
        var result = await sender.Send(new GetEmployeeLeaveBalancesQuery(companyId, year, employeeId));
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<UpsertEmployeeLeaveBalanceResult>> UpsertEmployeeLeaveBalance(
        [FromBody] UpsertEmployeeLeaveBalanceRequest request,
        ClaimsPrincipal user,
        ISender sender)
    {
        var result = await sender.Send(new UpsertEmployeeLeaveBalanceCommand(
            request.Balance,
            user.FindFirstValue(ClaimTypes.NameIdentifier)));
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<GetLeaveReportResult>> GetLeaveReport(
        [FromBody] GetLeaveReportRequest request,
        ISender sender)
    {
        var result = await sender.Send(new GetLeaveReportQuery(request.Filter));
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
