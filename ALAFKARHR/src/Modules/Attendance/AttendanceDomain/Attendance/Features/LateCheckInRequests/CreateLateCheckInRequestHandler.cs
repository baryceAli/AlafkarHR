using AttendanceDomain.Attendance.Models;
using AttendanceDomain.Attendance.Features;
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

        AttendanceLocationIntegrity.EnsureTrusted(
            request.Request.IsMockedLocation,
            request.Request.LocationIntegrityNote);

        var shiftWindow = await ResolveShiftWindowAsync(request.Request, employee, cancellationToken);

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

    private async Task<ShiftWindow> ResolveShiftWindowAsync(
        CreateLateCheckInRequestDto request,
        GetEmployeeAttendanceProfileResult employee,
        CancellationToken cancellationToken)
    {
        var workDateUtc = request.ShiftStart == default ? request.RequestedCheckInTimeUtc : DateTime.SpecifyKind(request.ShiftStart, DateTimeKind.Utc);
        var assignedShiftId = await ResolveAssignedShiftIdAsync(employee, workDateUtc, cancellationToken);
        var effectiveShiftId = assignedShiftId ?? request.ShiftId;

        if (!effectiveShiftId.HasValue)
        {
            return new ShiftWindow(
                null,
                DateTime.SpecifyKind(request.ShiftStart, DateTimeKind.Utc),
                DateTime.SpecifyKind(request.ShiftEnd, DateTimeKind.Utc),
                null);
        }

        var shift = await dbContext.Shifts
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == effectiveShiftId.Value && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("Shift", effectiveShiftId.Value);

        var shiftStart = shift.BuildShiftStart(workDateUtc);

        return new ShiftWindow(
            shift.Id,
            shiftStart,
            shift.BuildShiftEnd(workDateUtc),
            shift.ProhibitCheckInAfter(shiftStart));
    }

    private async Task<Guid?> ResolveAssignedShiftIdAsync(
        GetEmployeeAttendanceProfileResult employee,
        DateTime workDateUtc,
        CancellationToken cancellationToken)
    {
        var assignments = await dbContext.EmployeeShifts
            .AsNoTracking()
            .Where(x => x.IsActive
                && !x.IsDeleted
                && x.EffectiveFrom <= workDateUtc
                && (!x.EffectiveTo.HasValue || x.EffectiveTo.Value >= workDateUtc)
                && (
                    (x.Scope == ShiftAssignmentScope.Employee && x.EmployeeId == employee.EmployeeId)
                    || (x.Scope == ShiftAssignmentScope.Department && employee.DepartmentId.HasValue && x.DepartmentId == employee.DepartmentId.Value)
                    || (x.Scope == ShiftAssignmentScope.Administration && x.AdministrationId == employee.AdministrationId)
                    || (x.Scope == ShiftAssignmentScope.Company && x.CompanyId == employee.CompanyId)))
            .Select(x => new ShiftAssignmentCandidate(
                x.ShiftId,
                x.Scope,
                x.EffectiveFrom))
            .ToListAsync(cancellationToken);

        return assignments
            .OrderByDescending(x => Priority(x.Scope))
            .ThenByDescending(x => x.EffectiveFrom)
            .Select(x => (Guid?)x.ShiftId)
            .FirstOrDefault();
    }

    private static int Priority(ShiftAssignmentScope scope) => scope switch
    {
        ShiftAssignmentScope.Employee => 4,
        ShiftAssignmentScope.Department => 3,
        ShiftAssignmentScope.Administration => 2,
        ShiftAssignmentScope.Company => 1,
        _ => 0
    };

    private sealed record ShiftWindow(
        Guid? ShiftId,
        DateTime ShiftStart,
        DateTime ShiftEnd,
        DateTime? ProhibitCheckInAfterUtc);

    private sealed record ShiftAssignmentCandidate(
        Guid ShiftId,
        ShiftAssignmentScope Scope,
        DateTime EffectiveFrom);
}
