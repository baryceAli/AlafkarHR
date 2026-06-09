using AttendanceDomain.Attendance.Models;

namespace AttendanceDomain.Attendance.Features.ShiftAssignments;

public record AssignShiftCommand(AssignShiftDto Assignment) : ICommand<AssignShiftResult>;
public record AssignShiftResult(ShiftAssignmentDto Assignment);

public class AssignShiftHandler(AttendanceDbContext dbContext)
    : ICommandHandler<AssignShiftCommand, AssignShiftResult>
{
    public async Task<AssignShiftResult> Handle(AssignShiftCommand request, CancellationToken cancellationToken)
    {
        var shift = await dbContext.Shifts
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Assignment.ShiftId && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("Shift", request.Assignment.ShiftId);

        if (shift.CompanyId != request.Assignment.CompanyId)
        {
            throw new BadRequestException("Shift must belong to the assignment company.");
        }

        var effectiveFrom = request.Assignment.EffectiveFrom == default
            ? DateTime.UtcNow
            : UtcDateTime.Normalize(request.Assignment.EffectiveFrom);

        await CloseExistingAssignmentsAsync(request.Assignment, effectiveFrom, cancellationToken);

        var assignment = EmployeeShift.Assign(
            Guid.NewGuid(),
            request.Assignment.ShiftId,
            request.Assignment.Scope,
            request.Assignment.CompanyId,
            request.Assignment.AdministrationId,
            request.Assignment.DepartmentId,
            request.Assignment.EmployeeId,
            effectiveFrom,
            request.Assignment.EffectiveTo);

        await dbContext.EmployeeShifts.AddAsync(assignment, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new AssignShiftResult(assignment.Adapt<ShiftAssignmentDto>());
    }

    private async Task CloseExistingAssignmentsAsync(
        AssignShiftDto assignment,
        DateTime effectiveFrom,
        CancellationToken cancellationToken)
    {
        var existing = await dbContext.EmployeeShifts
            .Where(x => x.IsActive
                && !x.IsDeleted
                && x.Scope == assignment.Scope
                && x.CompanyId == assignment.CompanyId
                && x.EmployeeId == assignment.EmployeeId
                && x.DepartmentId == assignment.DepartmentId
                && x.AdministrationId == assignment.AdministrationId
                && (!x.EffectiveTo.HasValue || x.EffectiveTo.Value >= effectiveFrom))
            .ToListAsync(cancellationToken);

        foreach (var item in existing)
        {
            item.Close(effectiveFrom.AddTicks(-1));
        }
    }
}
