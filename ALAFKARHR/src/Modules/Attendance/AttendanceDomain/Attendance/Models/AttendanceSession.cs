using Shared.DDD;

namespace AttendanceDomain.Attendance.Models;

public class AttendanceSession : Aggregate<Guid>
{
    public Guid EmployeeId { get; private set; }
    public Guid? ShiftId { get; private set; }
    public EmployeeAttendanceType AttendanceType { get; private set; }
    public DateTime ShiftStart { get; private set; }
    public DateTime ShiftEnd { get; private set; }
    public DateTime? ActualStartTime { get; private set; }
    public DateTime? ActualEndTime { get; private set; }
    public AttendanceSessionStatus Status { get; private set; }
    public AttendanceNormalizationStatus NormalizationStatus { get; private set; }
    public decimal TotalHours { get; private set; }
    public decimal TotalDistanceKm { get; private set; }
    public Guid CompanyId { get; private set; }
    public string? NormalizationNote { get; private set; }
    public string? NormalizedBy { get; private set; }
    public DateTime? NormalizedAt { get; private set; }

    private AttendanceSession() { }

    public static AttendanceSession Start(
        Guid id,
        Guid employeeId,
        Guid companyId,
        Guid? shiftId,
        EmployeeAttendanceType attendanceType,
        DateTime shiftStart,
        DateTime shiftEnd,
        DateTime? actualStartTime = null)
    {
        var now = DateTime.UtcNow;
        var startTime = actualStartTime.HasValue
            ? UtcDateTime.Normalize(actualStartTime.Value)
            : now;

        return new AttendanceSession
        {
            Id = id,
            EmployeeId = employeeId,
            CompanyId = companyId,
            ShiftId = shiftId,
            AttendanceType = attendanceType,
            ShiftStart = UtcDateTime.Normalize(shiftStart),
            ShiftEnd = UtcDateTime.Normalize(shiftEnd),
            ActualStartTime = startTime,
            Status = AttendanceSessionStatus.Active,
            NormalizationStatus = AttendanceNormalizationStatus.Normal,
            CreatedAt = now
        };
    }

    public static AttendanceSession CompleteMissingCheckIn(
        Guid id,
        Guid employeeId,
        Guid companyId,
        Guid? shiftId,
        EmployeeAttendanceType attendanceType,
        DateTime shiftStart,
        DateTime shiftEnd,
        DateTime actualEndTime)
    {
        var now = DateTime.UtcNow;
        return new AttendanceSession
        {
            Id = id,
            EmployeeId = employeeId,
            CompanyId = companyId,
            ShiftId = shiftId,
            AttendanceType = attendanceType,
            ShiftStart = UtcDateTime.Normalize(shiftStart),
            ShiftEnd = UtcDateTime.Normalize(shiftEnd),
            ActualStartTime = null,
            ActualEndTime = UtcDateTime.Normalize(actualEndTime),
            Status = AttendanceSessionStatus.Completed,
            NormalizationStatus = AttendanceNormalizationStatus.MissingCheckIn,
            TotalHours = 0,
            CreatedAt = now,
            ModifiedAt = now
        };
    }

    public void StartBreak()
    {
        if (Status != AttendanceSessionStatus.Active)
        {
            throw new BadRequestException("Only an active attendance session can start a break.");
        }

        Status = AttendanceSessionStatus.OnBreak;
        ModifiedAt = DateTime.UtcNow;
    }

    public void EndBreak()
    {
        if (Status != AttendanceSessionStatus.OnBreak)
        {
            throw new BadRequestException("Only an on-break attendance session can end a break.");
        }

        Status = AttendanceSessionStatus.Active;
        ModifiedAt = DateTime.UtcNow;
    }

    public void AutoCloseMissingCheckOut()
    {
        if (Status is AttendanceSessionStatus.Completed or AttendanceSessionStatus.Cancelled)
        {
            return;
        }

        ActualEndTime = null;
        Status = AttendanceSessionStatus.Completed;
        NormalizationStatus = AttendanceNormalizationStatus.MissingCheckOut;
        TotalHours = CalculateHalfShiftHours();
        ModifiedAt = DateTime.UtcNow;
    }

    public void End(decimal totalDistanceKm)
    {
        if (Status is AttendanceSessionStatus.Completed or AttendanceSessionStatus.Cancelled)
        {
            throw new BadRequestException("Attendance session is already closed.");
        }

        ActualEndTime = DateTime.UtcNow;
        Status = AttendanceSessionStatus.Completed;
        TotalDistanceKm = decimal.Round(totalDistanceKm, 3);
        TotalHours = ActualStartTime.HasValue
            ? decimal.Round((decimal)(ActualEndTime.Value - ActualStartTime.Value).TotalHours, 2)
            : 0;
        ModifiedAt = DateTime.UtcNow;
    }

    public void Normalize(
        DateTime? checkInTimeUtc,
        DateTime? checkOutTimeUtc,
        bool markAbsent,
        string? managerNote,
        string normalizedBy)
    {
        if (Status is AttendanceSessionStatus.Active or AttendanceSessionStatus.OnBreak)
        {
            throw new BadRequestException("Active attendance sessions must be closed before normalization.");
        }

        var now = DateTime.UtcNow;
        if (markAbsent)
        {
            ActualStartTime = null;
            ActualEndTime = null;
            TotalHours = 0;
            NormalizationStatus = AttendanceNormalizationStatus.MarkedAbsent;
        }
        else
        {
            if (checkInTimeUtc.HasValue)
            {
                ActualStartTime = UtcDateTime.Normalize(checkInTimeUtc.Value);
            }

            if (checkOutTimeUtc.HasValue)
            {
                ActualEndTime = UtcDateTime.Normalize(checkOutTimeUtc.Value);
            }

            if (!ActualStartTime.HasValue)
            {
                NormalizationStatus = AttendanceNormalizationStatus.MissingCheckIn;
                TotalHours = 0;
            }
            else if (!ActualEndTime.HasValue)
            {
                NormalizationStatus = AttendanceNormalizationStatus.MissingCheckOut;
                TotalHours = CalculateHalfShiftHours();
            }
            else
            {
                if (ActualEndTime.Value <= ActualStartTime.Value)
                {
                    throw new BadRequestException("Check-out time must be after check-in time.");
                }

                NormalizationStatus = AttendanceNormalizationStatus.Normal;
                TotalHours = decimal.Round((decimal)(ActualEndTime.Value - ActualStartTime.Value).TotalHours, 2);
            }
        }

        NormalizationNote = managerNote;
        NormalizedBy = normalizedBy;
        NormalizedAt = now;
        ModifiedBy = normalizedBy;
        ModifiedAt = now;
    }

    private decimal CalculateHalfShiftHours()
    {
        var expectedHours = (decimal)(ShiftEnd - ShiftStart).TotalHours;
        if (expectedHours < 0)
        {
            expectedHours = 0;
        }

        return decimal.Round(expectedHours / 2, 2);
    }
}
