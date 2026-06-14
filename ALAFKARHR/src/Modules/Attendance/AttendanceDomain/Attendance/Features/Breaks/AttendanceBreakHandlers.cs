using AttendanceDomain.Attendance.Models;
using EmployeeModule.Contracts.Employees.Features.GetEmployeeAttendanceProfile;

namespace AttendanceDomain.Attendance.Features.Breaks;

public record StartAttendanceBreakCommand(Guid SessionId) : ICommand<AttendanceBreakResult>;
public record EndAttendanceBreakCommand(Guid SessionId) : ICommand<AttendanceBreakResult>;
public record AttendanceBreakResult(AttendanceSessionDto Session);

public class StartAttendanceBreakHandler(AttendanceDbContext dbContext, ISender sender)
    : ICommandHandler<StartAttendanceBreakCommand, AttendanceBreakResult>
{
    public async Task<AttendanceBreakResult> Handle(StartAttendanceBreakCommand request, CancellationToken cancellationToken)
    {
        var session = await dbContext.AttendanceSessions.FirstOrDefaultAsync(x => x.Id == request.SessionId, cancellationToken)
            ?? throw new NotFoundException("AttendanceSession", request.SessionId);

        await ValidateBreakPolicyAsync(session, cancellationToken);

        session.StartBreak();
        await dbContext.SaveChangesAsync(cancellationToken);

        return new AttendanceBreakResult(session.Adapt<AttendanceSessionDto>());
    }

    private async Task ValidateBreakPolicyAsync(AttendanceSession session, CancellationToken cancellationToken)
    {
        var employee = await sender.Send(new GetEmployeeAttendanceProfileQuery(session.EmployeeId), cancellationToken);
        var policies = await dbContext.AttendanceBreakPolicies
            .AsNoTracking()
            .Where(x => x.CompanyId == session.CompanyId && !x.IsDeleted
                && (
                    (x.Scope == ShiftAssignmentScope.Employee && x.EmployeeId == session.EmployeeId)
                    || (x.Scope == ShiftAssignmentScope.Department && employee.DepartmentId.HasValue && x.DepartmentId == employee.DepartmentId.Value)
                    || (x.Scope == ShiftAssignmentScope.Administration && x.AdministrationId == employee.AdministrationId)
                    || (x.Scope == ShiftAssignmentScope.Company && x.CompanyId == session.CompanyId)))
            .ToListAsync(cancellationToken);

        var policy = policies
            .OrderByDescending(x => x.Scope switch
            {
                ShiftAssignmentScope.Employee => 4,
                ShiftAssignmentScope.Department => 3,
                ShiftAssignmentScope.Administration => 2,
                ShiftAssignmentScope.Company => 1,
                _ => 0
            })
            .FirstOrDefault();

        if (policy is null)
        {
            return;
        }

        if (!policy.IsEnabled)
        {
            throw new BadRequestException("Break is disabled by the configured attendance break policy.");
        }

        if (policy.BreakMode != AttendanceBreakMode.Strict)
        {
            return;
        }

        if (!policy.BreakStartTime.HasValue || !policy.BreakEndTime.HasValue)
        {
            throw new BadRequestException("Strict break mode requires configured break start and end times.");
        }

        var now = DateTime.UtcNow.TimeOfDay;
        if (now < policy.BreakStartTime.Value || now > policy.BreakEndTime.Value)
        {
            throw new BadRequestException("Break can only be started during the configured strict break time.");
        }
    }
}

public class EndAttendanceBreakHandler(AttendanceDbContext dbContext)
    : ICommandHandler<EndAttendanceBreakCommand, AttendanceBreakResult>
{
    public async Task<AttendanceBreakResult> Handle(EndAttendanceBreakCommand request, CancellationToken cancellationToken)
    {
        var session = await dbContext.AttendanceSessions.FirstOrDefaultAsync(x => x.Id == request.SessionId, cancellationToken)
            ?? throw new NotFoundException("AttendanceSession", request.SessionId);

        session.EndBreak();
        await dbContext.SaveChangesAsync(cancellationToken);

        return new AttendanceBreakResult(session.Adapt<AttendanceSessionDto>());
    }
}
