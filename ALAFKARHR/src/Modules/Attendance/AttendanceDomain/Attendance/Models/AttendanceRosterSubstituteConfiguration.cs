using Shared.DDD;

namespace AttendanceDomain.Attendance.Models;

public class AttendanceRosterSubstituteConfiguration : Entity<Guid>
{
    public Guid CompanyId { get; private set; }
    public Guid EmployeeId { get; private set; }
    public bool IsRosterVisible { get; private set; } = true;
    public bool IsSubstituteEligible { get; private set; } = true;
    public string? Notes { get; private set; }

    private AttendanceRosterSubstituteConfiguration() { }

    public static AttendanceRosterSubstituteConfiguration Create(
        Guid id,
        Guid companyId,
        Guid employeeId,
        bool isRosterVisible,
        bool isSubstituteEligible,
        string? notes,
        string? createdBy)
    {
        Validate(companyId, employeeId);

        return new AttendanceRosterSubstituteConfiguration
        {
            Id = id,
            CompanyId = companyId,
            EmployeeId = employeeId,
            IsRosterVisible = isRosterVisible,
            IsSubstituteEligible = isSubstituteEligible,
            Notes = NormalizeNotes(notes),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void Update(
        bool isRosterVisible,
        bool isSubstituteEligible,
        string? notes,
        string? modifiedBy)
    {
        IsRosterVisible = isRosterVisible;
        IsSubstituteEligible = isSubstituteEligible;
        Notes = NormalizeNotes(notes);
        ModifiedBy = modifiedBy;
        ModifiedAt = DateTime.UtcNow;
    }

    private static void Validate(Guid companyId, Guid employeeId)
    {
        if (companyId == Guid.Empty)
        {
            throw new BadRequestException("Company is required for roster substitute configuration.");
        }

        if (employeeId == Guid.Empty)
        {
            throw new BadRequestException("Employee is required for roster substitute configuration.");
        }
    }

    private static string? NormalizeNotes(string? notes)
        => string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
}
