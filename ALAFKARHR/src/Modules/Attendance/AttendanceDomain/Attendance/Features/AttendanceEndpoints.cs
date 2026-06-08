using AttendanceDomain.Attendance.Features.Breaks;
using AttendanceDomain.Attendance.Features.CheckIns;
using AttendanceDomain.Attendance.Features.EndSession;
using AttendanceDomain.Attendance.Features.LocationPings;
using AttendanceDomain.Attendance.Features.StartSession;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceDomain.Attendance.Features;

public record StartAttendanceSessionRequest(StartAttendanceSessionDto Session);
public record EndAttendanceSessionRequest(EndAttendanceSessionDto Session);
public record AttendanceBreakRequest(Guid SessionId);
public record SubmitAttendanceLocationPingRequest(AttendanceLocationPingDto Ping);
public record SubmitAttendanceLocationPingBatchRequest(IReadOnlyCollection<AttendanceLocationPingDto> Pings);
public record CreateAttendanceCheckInRequest(AttendanceCheckInDto CheckIn);

public class AttendanceEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/attendance")
            .RequireAuthorization()
            .WithTags("Attendance");

        group.MapPost("/sessions/start", StartSession)
            .WithName("StartAttendanceSession")
            .Produces<StartAttendanceSessionResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Start an attendance session")
            .WithDescription("Starts attendance for fixed-location or mobile employees. Fixed-location employees are validated against the assigned department geofence.");

        group.MapPost("/sessions/end", EndSession)
            .WithName("EndAttendanceSession")
            .Produces<EndAttendanceSessionResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("End an attendance session");

        group.MapPost("/sessions/break/start", StartBreak)
            .WithName("StartAttendanceBreak")
            .Produces<AttendanceBreakResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Pause attendance tracking for a break");

        group.MapPost("/sessions/break/end", EndBreak)
            .WithName("EndAttendanceBreak")
            .Produces<AttendanceBreakResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Resume attendance tracking after a break");

        group.MapPost("/location/ping", SubmitLocationPing)
            .WithName("SubmitAttendanceLocationPing")
            .Produces<SubmitAttendanceLocationPingResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Submit a GPS ping for an active session");

        group.MapPost("/location/ping/batch", SubmitLocationPingBatch)
            .WithName("SubmitAttendanceLocationPingBatch")
            .Produces<SubmitAttendanceLocationPingBatchResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Batch-sync offline GPS pings");

        group.MapPost("/checkins", CreateCheckIn)
            .WithName("CreateAttendanceCheckIn")
            .Produces<CreateAttendanceCheckInResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create a mobile site or task check-in");
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
}
