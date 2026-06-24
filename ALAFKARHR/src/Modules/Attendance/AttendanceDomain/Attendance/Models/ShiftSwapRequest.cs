using Shared.DDD;

namespace AttendanceDomain.Attendance.Models;

public class ShiftSwapRequest : Entity<Guid>
{
    public Guid CompanyId { get; private set; }
    public Guid? ScheduleAssignmentId { get; private set; }
    public Guid RequestingEmployeeId { get; private set; }
    public Guid TargetEmployeeId { get; private set; }
    public DateTime WorkDate { get; private set; }
    public Guid? RequestedShiftId { get; private set; }
    public AttendanceExceptionStatus Status { get; private set; }
    public string? Reason { get; private set; }
    public string? ManagerNote { get; private set; }
    public string? ReviewedBy { get; private set; }
    public DateTime? ReviewedAtUtc { get; private set; }

    private ShiftSwapRequest() { }

    public static ShiftSwapRequest Create(Guid id, CreateShiftSwapRequestDto dto)
    {
        if (dto.CompanyId == Guid.Empty || dto.RequestingEmployeeId == Guid.Empty || dto.TargetEmployeeId == Guid.Empty)
        {
            throw new BadRequestException("Company, requesting employee, and target employee are required for shift swap.");
        }

        if (dto.RequestingEmployeeId == dto.TargetEmployeeId)
        {
            throw new BadRequestException("Requesting and target employees must be different.");
        }

        return new ShiftSwapRequest
        {
            Id = id,
            CompanyId = dto.CompanyId,
            ScheduleAssignmentId = dto.ScheduleAssignmentId,
            RequestingEmployeeId = dto.RequestingEmployeeId,
            TargetEmployeeId = dto.TargetEmployeeId,
            WorkDate = UtcDateTime.Normalize(dto.WorkDate).Date,
            RequestedShiftId = dto.RequestedShiftId,
            Reason = dto.Reason,
            Status = AttendanceExceptionStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Review(bool isApproved, string? managerNote, string? reviewedBy)
    {
        if (Status != AttendanceExceptionStatus.Pending)
        {
            throw new BadRequestException("Only pending swap requests can be reviewed.");
        }

        Status = isApproved ? AttendanceExceptionStatus.Approved : AttendanceExceptionStatus.Rejected;
        ManagerNote = managerNote;
        ReviewedBy = reviewedBy;
        ReviewedAtUtc = DateTime.UtcNow;
        ModifiedBy = reviewedBy;
        ModifiedAt = DateTime.UtcNow;
    }

    public void Cancel(string? userId)
    {
        if (Status != AttendanceExceptionStatus.Pending)
        {
            throw new BadRequestException("Only pending swap requests can be cancelled.");
        }

        Status = AttendanceExceptionStatus.Cancelled;
        ModifiedBy = userId;
        ModifiedAt = DateTime.UtcNow;
    }
}
