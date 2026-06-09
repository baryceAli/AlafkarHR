using Shared.DDD;

namespace AttendanceDomain.Attendance.Models;

public class LateCheckInRequest : Entity<Guid>
{
    public Guid EmployeeId { get; private set; }
    public Guid CompanyId { get; private set; }
    public Guid? ShiftId { get; private set; }
    public Guid? SessionId { get; private set; }
    public EmployeeAttendanceType AttendanceType { get; private set; }
    public DateTime ShiftStart { get; private set; }
    public DateTime ShiftEnd { get; private set; }
    public DateTime RequestedCheckInTimeUtc { get; private set; }
    public DateTime? RegisteredCheckInTimeUtc { get; private set; }
    public string Reason { get; private set; }
    public AttendanceExceptionStatus Status { get; private set; }
    public string? ManagerNote { get; private set; }
    public string? ReviewedBy { get; private set; }
    public DateTime? ReviewedAt { get; private set; }
    public double? Latitude { get; private set; }
    public double? Longitude { get; private set; }
    public double? AccuracyMeters { get; private set; }

    private LateCheckInRequest() { }

    public static LateCheckInRequest Create(
        Guid id,
        Guid employeeId,
        Guid companyId,
        Guid? shiftId,
        EmployeeAttendanceType attendanceType,
        DateTime shiftStart,
        DateTime shiftEnd,
        DateTime requestedCheckInTimeUtc,
        string reason,
        double? latitude,
        double? longitude,
        double? accuracyMeters)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new BadRequestException("Reason is required for a late check-in request.");
        }

        return new LateCheckInRequest
        {
            Id = id,
            EmployeeId = employeeId,
            CompanyId = companyId,
            ShiftId = shiftId,
            AttendanceType = attendanceType,
            ShiftStart = UtcDateTime.Normalize(shiftStart),
            ShiftEnd = UtcDateTime.Normalize(shiftEnd),
            RequestedCheckInTimeUtc = UtcDateTime.Normalize(requestedCheckInTimeUtc),
            Reason = reason.Trim(),
            Status = AttendanceExceptionStatus.Pending,
            Latitude = latitude,
            Longitude = longitude,
            AccuracyMeters = accuracyMeters,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Approve(Guid sessionId, DateTime registeredCheckInTimeUtc, string? managerNote, string reviewedBy)
    {
        EnsurePending();

        SessionId = sessionId;
        RegisteredCheckInTimeUtc = UtcDateTime.Normalize(registeredCheckInTimeUtc);
        Status = AttendanceExceptionStatus.Approved;
        ManagerNote = managerNote;
        ReviewedBy = reviewedBy;
        ReviewedAt = DateTime.UtcNow;
        ModifiedAt = ReviewedAt;
        ModifiedBy = reviewedBy;
    }

    public void Reject(string? managerNote, string reviewedBy)
    {
        EnsurePending();

        Status = AttendanceExceptionStatus.Rejected;
        ManagerNote = managerNote;
        ReviewedBy = reviewedBy;
        ReviewedAt = DateTime.UtcNow;
        ModifiedAt = ReviewedAt;
        ModifiedBy = reviewedBy;
    }

    private void EnsurePending()
    {
        if (Status != AttendanceExceptionStatus.Pending)
        {
            throw new BadRequestException("Late check-in request has already been reviewed.");
        }
    }
}
