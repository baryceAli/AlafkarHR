using AttendanceDomain.Attendance.Models;

namespace AttendanceDomain.Attendance.Features.LateCheckInRequests;

public record ReviewLateCheckInRequestCommand(ReviewLateCheckInRequestDto Review, string ReviewedBy)
    : ICommand<ReviewLateCheckInRequestResult>;

public record ReviewLateCheckInRequestResult(LateCheckInRequestDto Request, AttendanceSessionDto? Session);

public class ReviewLateCheckInRequestHandler(AttendanceDbContext dbContext)
    : ICommandHandler<ReviewLateCheckInRequestCommand, ReviewLateCheckInRequestResult>
{
    public async Task<ReviewLateCheckInRequestResult> Handle(
        ReviewLateCheckInRequestCommand request,
        CancellationToken cancellationToken)
    {
        var lateRequest = await dbContext.LateCheckInRequests
            .FirstOrDefaultAsync(x => x.Id == request.Review.RequestId, cancellationToken)
            ?? throw new NotFoundException("LateCheckInRequest", request.Review.RequestId);

        if (!request.Review.IsApproved)
        {
            lateRequest.Reject(request.Review.ManagerNote, request.ReviewedBy);
            await dbContext.SaveChangesAsync(cancellationToken);

            return new ReviewLateCheckInRequestResult(lateRequest.Adapt<LateCheckInRequestDto>(), null);
        }

        var registeredTime = request.Review.RegisteredCheckInTimeUtc ?? lateRequest.RequestedCheckInTimeUtc;

        var workdayStartUtc = lateRequest.ShiftStart.Date;
        var workdayEndUtc = workdayStartUtc.AddDays(1);

        if (await AttendanceSessionLifecycle.AutoCloseStaleSessionsAsync(
            dbContext,
            lateRequest.EmployeeId,
            workdayStartUtc,
            cancellationToken))
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        var activeSessions = await dbContext.AttendanceSessions
            .Where(x => x.EmployeeId == lateRequest.EmployeeId
                && (x.Status == AttendanceSessionStatus.Active || x.Status == AttendanceSessionStatus.OnBreak))
            .ToListAsync(cancellationToken);

        if (activeSessions.Any(x => AttendanceSessionLifecycle.IsCurrentWorkdaySession(x, workdayStartUtc, workdayEndUtc)))
        {
            throw new BadRequestException("Employee already has an active attendance session.");
        }

        var session = AttendanceSession.Start(
            Guid.NewGuid(),
            lateRequest.EmployeeId,
            lateRequest.CompanyId,
            lateRequest.ShiftId,
            lateRequest.AttendanceType,
            lateRequest.ShiftStart,
            lateRequest.ShiftEnd,
            registeredTime);

        lateRequest.Approve(session.Id, registeredTime, request.Review.ManagerNote, request.ReviewedBy);

        await dbContext.AttendanceSessions.AddAsync(session, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new ReviewLateCheckInRequestResult(
            lateRequest.Adapt<LateCheckInRequestDto>(),
            session.Adapt<AttendanceSessionDto>());
    }
}
