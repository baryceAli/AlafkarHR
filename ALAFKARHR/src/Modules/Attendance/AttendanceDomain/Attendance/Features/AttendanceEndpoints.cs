using AttendanceDomain.Attendance.Features.Breaks;
using AttendanceDomain.Attendance.Features.CheckIns;
using AttendanceDomain.Attendance.Features.EndSession;
using AttendanceDomain.Attendance.Features.LateCheckInRequests;
using AttendanceDomain.Attendance.Features.LocationPings;
using AttendanceDomain.Attendance.Features.Queries;
using AttendanceDomain.Attendance.Features.ShiftAssignments;
using AttendanceDomain.Attendance.Features.Shifts;
using AttendanceDomain.Attendance.Features.StartSession;
using EmployeeModule.Contracts.Employees.Features.GetEmployeeAttendanceProfile;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Pagination;
using SharedWithUI.Permissions;
using System.Security.Claims;

namespace AttendanceDomain.Attendance.Features;

public record StartAttendanceSessionRequest(StartAttendanceSessionDto Session);
public record EndAttendanceSessionRequest(EndAttendanceSessionDto Session);
public record AttendanceBreakRequest(Guid SessionId);
public record SubmitAttendanceLocationPingRequest(AttendanceLocationPingDto Ping);
public record SubmitAttendanceLocationPingBatchRequest(IReadOnlyCollection<AttendanceLocationPingDto> Pings);
public record CreateAttendanceCheckInRequest(AttendanceCheckInDto CheckIn);
public record CreateLateCheckInRequestRequest(CreateLateCheckInRequestDto Request);
public record ReviewLateCheckInRequestRequest(ReviewLateCheckInRequestDto Review);
public record AssignShiftRequest(AssignShiftDto Assignment);
public record CreateShiftRequest(CreateShiftDto Shift);
public record UpdateShiftRequest(ShiftDto Shift);

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
        [FromQuery] bool isMockedLocation,
        [FromQuery] string? locationIntegrityNote,
        [FromQuery] DateTime? workDateUtc,
        ISender sender)
    {
        var result = await sender.Send(new GetAttendanceCheckInPreviewQuery(
            employeeId,
            latitude,
            longitude,
            accuracyMeters,
            isMockedLocation,
            locationIntegrityNote,
            workDateUtc));

        return TypedResults.Ok(result);
    }

    private static async Task<Ok<GetAttendanceCheckInPreviewResult>> GetMyCheckInPreview(
        [FromQuery] double? latitude,
        [FromQuery] double? longitude,
        [FromQuery] double? accuracyMeters,
        [FromQuery] bool isMockedLocation,
        [FromQuery] string? locationIntegrityNote,
        [FromQuery] DateTime? workDateUtc,
        ClaimsPrincipal user,
        ISender sender)
    {
        var employeeId = ResolveEmployeeIdClaim(user);
        if (!employeeId.HasValue)
        {
            var userName = FirstClaimValue(
                user,
                ClaimTypes.Name,
                "name",
                "unique_name",
                "preferred_username",
                "upn")
                ?? throw new UnauthorizedAccessException("The signed-in user does not have a username claim that can be matched to an employee code.");

            var employee = await sender.Send(new GetEmployeeAttendanceProfileByCodeQuery(userName));
            employeeId = employee.EmployeeId;
        }

        var result = await sender.Send(new GetAttendanceCheckInPreviewQuery(
            employeeId.Value,
            latitude,
            longitude,
            accuracyMeters,
            isMockedLocation,
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
