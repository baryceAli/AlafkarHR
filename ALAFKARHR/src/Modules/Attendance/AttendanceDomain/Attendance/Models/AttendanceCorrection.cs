using Shared.DDD;

namespace AttendanceDomain.Attendance.Models;

public class AttendanceCorrection : Entity<Guid>
{
    public Guid CompanyId { get; private set; }
    public Guid EmployeeId { get; private set; }
    public Guid? SessionId { get; private set; }
    public DateTime WorkDate { get; private set; }
    public DateTime? CurrentCheckInUtc { get; private set; }
    public DateTime? CurrentCheckOutUtc { get; private set; }
    public AttendanceSessionStatus? CurrentSessionStatus { get; private set; }
    public DateTime? CorrectedCheckInUtc { get; private set; }
    public DateTime? CorrectedCheckOutUtc { get; private set; }
    public AttendanceExceptionStatus Status { get; private set; }
    public string? Reason { get; private set; }
    public string? ManagerNote { get; private set; }
    public string? ReviewedBy { get; private set; }
    public DateTime? ReviewedAtUtc { get; private set; }
    public DateTime? AppliedAtUtc { get; private set; }

    private AttendanceCorrection() { }

    public static AttendanceCorrection Create(Guid id, CreateAttendanceCorrectionDto dto, AttendanceSession? currentSession)
    {
        if (dto.CompanyId == Guid.Empty || dto.EmployeeId == Guid.Empty)
        {
            throw new BadRequestException("Company and employee are required for attendance correction.");
        }

        if (dto.CorrectedCheckInUtc.HasValue && dto.CorrectedCheckOutUtc.HasValue
            && dto.CorrectedCheckOutUtc.Value <= dto.CorrectedCheckInUtc.Value)
        {
            throw new BadRequestException("Corrected checkout must be after corrected check-in.");
        }

        return new AttendanceCorrection
        {
            Id = id,
            CompanyId = dto.CompanyId,
            EmployeeId = dto.EmployeeId,
            SessionId = currentSession?.Id ?? dto.SessionId,
            WorkDate = UtcDateTime.Normalize(dto.WorkDate).Date,
            CurrentCheckInUtc = UtcDateTime.Normalize(currentSession?.ActualStartTime),
            CurrentCheckOutUtc = UtcDateTime.Normalize(currentSession?.ActualEndTime),
            CurrentSessionStatus = currentSession?.Status,
            CorrectedCheckInUtc = UtcDateTime.Normalize(dto.CorrectedCheckInUtc),
            CorrectedCheckOutUtc = UtcDateTime.Normalize(dto.CorrectedCheckOutUtc),
            Reason = dto.Reason,
            Status = AttendanceExceptionStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Review(bool isApproved, string? managerNote, string? reviewedBy)
    {
        if (Status != AttendanceExceptionStatus.Pending)
        {
            throw new BadRequestException("Only pending corrections can be reviewed.");
        }

        Status = isApproved ? AttendanceExceptionStatus.Approved : AttendanceExceptionStatus.Rejected;
        ManagerNote = managerNote;
        ReviewedBy = reviewedBy;
        ReviewedAtUtc = DateTime.UtcNow;
        ModifiedBy = reviewedBy;
        ModifiedAt = DateTime.UtcNow;
    }

    public void MarkApplied(string? userId)
    {
        if (Status != AttendanceExceptionStatus.Approved)
        {
            throw new BadRequestException("Only approved corrections can be applied.");
        }

        if (AppliedAtUtc.HasValue)
        {
            throw new BadRequestException("Correction has already been applied.");
        }

        AppliedAtUtc = DateTime.UtcNow;
        ModifiedBy = userId;
        ModifiedAt = DateTime.UtcNow;
    }
}
