using System.Security.Claims;

namespace AttendanceDomain.Attendance.Features.Shifts;

public record DeleteShiftCommand(Guid ShiftId) : ICommand<DeleteShiftResult>;
public record DeleteShiftResult(bool IsSuccess);

public class DeleteShiftHandler(AttendanceDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<DeleteShiftCommand, DeleteShiftResult>
{
    public async Task<DeleteShiftResult> Handle(DeleteShiftCommand request, CancellationToken cancellationToken)
    {
        var shift = await dbContext.Shifts
            .FirstOrDefaultAsync(x => x.Id == request.ShiftId && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("Shift", request.ShiftId);

        var userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        shift.Delete(userId);

        var activeAssignments = await dbContext.EmployeeShifts
            .Where(x => x.ShiftId == request.ShiftId && x.IsActive && !x.IsDeleted)
            .ToListAsync(cancellationToken);

        var closedAt = DateTime.UtcNow.AddTicks(-1);
        foreach (var assignment in activeAssignments)
        {
            assignment.Close(closedAt);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return new DeleteShiftResult(true);
    }
}
