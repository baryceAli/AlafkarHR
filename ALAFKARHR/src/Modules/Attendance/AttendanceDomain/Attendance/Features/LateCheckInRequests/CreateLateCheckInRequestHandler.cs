using AttendanceDomain.Attendance.Models;
using EmployeeModule.Contracts.Employees.Features.GetEmployeeAttendanceProfile;

namespace AttendanceDomain.Attendance.Features.LateCheckInRequests;

public record CreateLateCheckInRequestCommand(CreateLateCheckInRequestDto Request)
    : ICommand<CreateLateCheckInRequestResult>;

public record CreateLateCheckInRequestResult(LateCheckInRequestDto Request);

public class CreateLateCheckInRequestHandler(AttendanceDbContext dbContext, ISender sender)
    : ICommandHandler<CreateLateCheckInRequestCommand, CreateLateCheckInRequestResult>
{
    public async Task<CreateLateCheckInRequestResult> Handle(
        CreateLateCheckInRequestCommand request,
        CancellationToken cancellationToken)
    {
        var employee = await sender.Send(
            new GetEmployeeAttendanceProfileQuery(request.Request.EmployeeId),
            cancellationToken);

        if (!employee.IsActive)
        {
            throw new BadRequestException("Inactive employees cannot submit late check-in requests.");
        }

        var shiftWindow = await ResolveShiftWindowAsync(request.Request, cancellationToken);

        if (shiftWindow.ProhibitCheckInAfterUtc.HasValue &&
            request.Request.RequestedCheckInTimeUtc <= shiftWindow.ProhibitCheckInAfterUtc.Value)
        {
            throw new BadRequestException("Late check-in requests are only required after the check-in prohibition time.");
        }

        var pendingRequestExists = await dbContext.LateCheckInRequests.AnyAsync(
            x => x.EmployeeId == request.Request.EmployeeId
                && x.Status == AttendanceExceptionStatus.Pending
                && x.ShiftStart == shiftWindow.ShiftStart,
            cancellationToken);

        if (pendingRequestExists)
        {
            throw new BadRequestException("A pending late check-in request already exists for this shift.");
        }

        var lateRequest = LateCheckInRequest.Create(
            Guid.NewGuid(),
            employee.EmployeeId,
            employee.CompanyId,
            shiftWindow.ShiftId,
            employee.AttendanceType,
            shiftWindow.ShiftStart,
            shiftWindow.ShiftEnd,
            request.Request.RequestedCheckInTimeUtc,
            request.Request.Reason,
            request.Request.Latitude,
            request.Request.Longitude,
            request.Request.AccuracyMeters);

        await dbContext.LateCheckInRequests.AddAsync(lateRequest, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateLateCheckInRequestResult(lateRequest.Adapt<LateCheckInRequestDto>());
    }

    private async Task<ShiftWindow> ResolveShiftWindowAsync(CreateLateCheckInRequestDto request, CancellationToken cancellationToken)
    {
        if (!request.ShiftId.HasValue)
        {
            return new ShiftWindow(
                null,
                DateTime.SpecifyKind(request.ShiftStart, DateTimeKind.Utc),
                DateTime.SpecifyKind(request.ShiftEnd, DateTimeKind.Utc),
                null);
        }

        var shift = await dbContext.Shifts
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.ShiftId.Value, cancellationToken)
            ?? throw new NotFoundException("Shift", request.ShiftId.Value);

        var workDateUtc = request.ShiftStart == default ? request.RequestedCheckInTimeUtc : request.ShiftStart;
        var shiftStart = shift.BuildShiftStart(workDateUtc);

        return new ShiftWindow(
            shift.Id,
            shiftStart,
            shift.BuildShiftEnd(workDateUtc),
            shift.ProhibitCheckInAfter(shiftStart));
    }

    private sealed record ShiftWindow(
        Guid? ShiftId,
        DateTime ShiftStart,
        DateTime ShiftEnd,
        DateTime? ProhibitCheckInAfterUtc);
}
