using AttendanceDomain.Attendance.Models;
using AttendanceDomain.Attendance.Features;

namespace AttendanceDomain.Attendance.Features.CheckIns;

public record CreateAttendanceCheckInCommand(AttendanceCheckInDto CheckIn)
    : ICommand<CreateAttendanceCheckInResult>;

public record CreateAttendanceCheckInResult(bool IsSuccess, bool IsDuplicate);

public class CreateAttendanceCheckInHandler(AttendanceDbContext dbContext)
    : ICommandHandler<CreateAttendanceCheckInCommand, CreateAttendanceCheckInResult>
{
    public async Task<CreateAttendanceCheckInResult> Handle(
        CreateAttendanceCheckInCommand request,
        CancellationToken cancellationToken)
    {
        AttendanceLocationIntegrity.EnsureTrusted(
            request.CheckIn.IsMockedLocation,
            request.CheckIn.LocationIntegrityNote);

        if (request.CheckIn.ClientCheckInId.HasValue)
        {
            var exists = await dbContext.AttendanceCheckIns.AnyAsync(
                x => x.ClientCheckInId == request.CheckIn.ClientCheckInId,
                cancellationToken);

            if (exists)
            {
                return new CreateAttendanceCheckInResult(true, true);
            }
        }

        var session = await dbContext.AttendanceSessions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.CheckIn.SessionId, cancellationToken)
            ?? throw new NotFoundException("AttendanceSession", request.CheckIn.SessionId);

        if (session.EmployeeId != request.CheckIn.EmployeeId)
        {
            throw new BadRequestException("Check-in employee does not match the attendance session.");
        }

        if (session.Status != AttendanceSessionStatus.Active)
        {
            throw new BadRequestException("Check-ins are accepted only while the session is active.");
        }

        await dbContext.AttendanceCheckIns.AddAsync(
            AttendanceCheckIn.Create(
                Guid.NewGuid(),
                request.CheckIn.ClientCheckInId,
                request.CheckIn.SessionId,
                request.CheckIn.EmployeeId,
                request.CheckIn.SiteName,
                request.CheckIn.Latitude,
                request.CheckIn.Longitude,
                request.CheckIn.ArrivedAtUtc,
                request.CheckIn.DepartedAtUtc,
                request.CheckIn.Notes),
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateAttendanceCheckInResult(true, false);
    }
}
