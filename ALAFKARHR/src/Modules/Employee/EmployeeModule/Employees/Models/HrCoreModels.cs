using Shared.DDD;
using Shared.Exceptions;
using SharedWithUI.HRCore.Dtos;

namespace EmployeeModule.Employees.Models;

public class HrLifecycleEvent : Entity<Guid>
{
    public Guid EmployeeId { get; private set; }
    public Guid CompanyId { get; private set; }
    public HrLifecycleEventType EventType { get; private set; }
    public HrLifecycleEventStatus Status { get; private set; }
    public DateTime EffectiveDate { get; private set; }
    public Guid? FromBranchId { get; private set; }
    public Guid? ToBranchId { get; private set; }
    public Guid? FromDepartmentId { get; private set; }
    public Guid? ToDepartmentId { get; private set; }
    public Guid? FromPositionId { get; private set; }
    public Guid? ToPositionId { get; private set; }
    public Guid? FromManagerEmployeeId { get; private set; }
    public Guid? ToManagerEmployeeId { get; private set; }
    public string? FromGrade { get; private set; }
    public string? ToGrade { get; private set; }
    public string? FromWorkLocation { get; private set; }
    public string? ToWorkLocation { get; private set; }
    public string? Reason { get; private set; }
    public string? Notes { get; private set; }
    public DateTime? SubmittedAt { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }

    private HrLifecycleEvent() { }

    public static HrLifecycleEvent Create(HrLifecycleEventDto dto, Employee employee, string createdBy)
    {
        return new HrLifecycleEvent
        {
            Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
            EmployeeId = employee.Id,
            CompanyId = employee.CompanyId,
            EventType = dto.EventType,
            Status = HrLifecycleEventStatus.Draft,
            EffectiveDate = dto.EffectiveDate == default ? DateTime.UtcNow.Date : dto.EffectiveDate.Date,
            FromBranchId = employee.BranchId,
            ToBranchId = dto.ToBranchId,
            FromDepartmentId = employee.DepartmentId,
            ToDepartmentId = dto.ToDepartmentId,
            FromPositionId = employee.PositionId,
            ToPositionId = dto.ToPositionId,
            FromManagerEmployeeId = employee.ManagerEmployeeId,
            ToManagerEmployeeId = dto.ToManagerEmployeeId,
            FromGrade = Normalize(employee.Grade),
            ToGrade = Normalize(dto.ToGrade),
            FromWorkLocation = Normalize(employee.WorkLocation),
            ToWorkLocation = Normalize(dto.ToWorkLocation),
            Reason = Normalize(dto.Reason),
            Notes = Normalize(dto.Notes),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void Update(HrLifecycleEventDto dto, Employee employee, string modifiedBy)
    {
        EnsureEditable();
        EventType = dto.EventType;
        EffectiveDate = dto.EffectiveDate == default ? EffectiveDate : dto.EffectiveDate.Date;
        FromBranchId = employee.BranchId;
        ToBranchId = dto.ToBranchId;
        FromDepartmentId = employee.DepartmentId;
        ToDepartmentId = dto.ToDepartmentId;
        FromPositionId = employee.PositionId;
        ToPositionId = dto.ToPositionId;
        FromManagerEmployeeId = employee.ManagerEmployeeId;
        ToManagerEmployeeId = dto.ToManagerEmployeeId;
        FromGrade = Normalize(employee.Grade);
        ToGrade = Normalize(dto.ToGrade);
        FromWorkLocation = Normalize(employee.WorkLocation);
        ToWorkLocation = Normalize(dto.ToWorkLocation);
        Reason = Normalize(dto.Reason);
        Notes = Normalize(dto.Notes);
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void Submit(string modifiedBy)
    {
        if (Status != HrLifecycleEventStatus.Draft)
            throw new BadRequestException("Only draft lifecycle events can be submitted.");

        Status = HrLifecycleEventStatus.Submitted;
        SubmittedAt = DateTime.UtcNow;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void Approve(string modifiedBy)
    {
        if (Status != HrLifecycleEventStatus.Submitted)
            throw new BadRequestException("Only submitted lifecycle events can be approved.");

        Status = HrLifecycleEventStatus.Approved;
        ApprovedAt = DateTime.UtcNow;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void Complete(Employee employee, string modifiedBy)
    {
        if (Status != HrLifecycleEventStatus.Approved)
            throw new BadRequestException("Only approved lifecycle events can be completed.");

        if (EventType is HrLifecycleEventType.Transfer && ToBranchId.HasValue)
        {
            employee.TransferDepartment(ToBranchId.Value, employee.AdministrationId, ToDepartmentId ?? employee.DepartmentId, modifiedBy);
        }

        if (EventType is HrLifecycleEventType.Promotion && ToPositionId.HasValue)
        {
            employee.ChangePosition(ToPositionId.Value, modifiedBy);
        }

        if (EventType is HrLifecycleEventType.Transfer or HrLifecycleEventType.Promotion)
        {
            employee.UpdateHrPlacement(
                ToManagerEmployeeId ?? employee.ManagerEmployeeId,
                ToGrade ?? employee.Grade,
                ToWorkLocation ?? employee.WorkLocation,
                employee.LinkedUserId,
                modifiedBy);
        }

        if (EventType is HrLifecycleEventType.Separation)
        {
            employee.Terminate(Reason ?? "Lifecycle separation", modifiedBy);
        }

        Status = HrLifecycleEventStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void Cancel(string modifiedBy)
    {
        if (Status == HrLifecycleEventStatus.Completed)
            throw new BadRequestException("Completed lifecycle events cannot be cancelled.");

        Status = HrLifecycleEventStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void Remove(string deletedBy)
    {
        EnsureEditable();
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    private void EnsureEditable()
    {
        if (Status is HrLifecycleEventStatus.Completed or HrLifecycleEventStatus.Cancelled)
            throw new BadRequestException("Completed or cancelled lifecycle events cannot be edited.");
    }

    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public class EmployeeEmergencyContact : Entity<Guid>
{
    public Guid EmployeeId { get; private set; }
    public Guid CompanyId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Relationship { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public string? Email { get; private set; }
    public bool IsPrimary { get; private set; }

    private EmployeeEmergencyContact() { }

    public static EmployeeEmergencyContact Create(EmployeeEmergencyContactDto dto, Employee employee, string createdBy)
        => new()
        {
            Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
            EmployeeId = employee.Id,
            CompanyId = employee.CompanyId,
            Name = Required(dto.Name, "Contact name is required."),
            Relationship = Required(dto.Relationship, "Relationship is required."),
            Phone = Required(dto.Phone, "Phone is required."),
            Email = Normalize(dto.Email),
            IsPrimary = dto.IsPrimary,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

    public void Update(EmployeeEmergencyContactDto dto, string modifiedBy)
    {
        Name = Required(dto.Name, "Contact name is required.");
        Relationship = Required(dto.Relationship, "Relationship is required.");
        Phone = Required(dto.Phone, "Phone is required.");
        Email = Normalize(dto.Email);
        IsPrimary = dto.IsPrimary;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void Remove(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    private static string Required(string? value, string message) => string.IsNullOrWhiteSpace(value) ? throw new BadRequestException(message) : value.Trim();
    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public class EmployeeDocumentLink : Entity<Guid>
{
    public Guid EmployeeId { get; private set; }
    public Guid CompanyId { get; private set; }
    public Guid? DocumentId { get; private set; }
    public string DocumentType { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public DateTime? IssueDate { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    public string? Notes { get; private set; }

    private EmployeeDocumentLink() { }

    public static EmployeeDocumentLink Create(EmployeeDocumentLinkDto dto, Employee employee, string createdBy)
    {
        var link = new EmployeeDocumentLink
        {
            Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
            EmployeeId = employee.Id,
            CompanyId = employee.CompanyId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
        link.Update(dto, createdBy);
        link.CreatedAt = DateTime.UtcNow;
        link.CreatedBy = createdBy;
        return link;
    }

    public void Update(EmployeeDocumentLinkDto dto, string modifiedBy)
    {
        DocumentId = dto.DocumentId;
        DocumentType = Required(dto.DocumentType, "Document type is required.");
        Title = Required(dto.Title, "Document title is required.");
        IssueDate = dto.IssueDate?.Date;
        ExpiryDate = dto.ExpiryDate?.Date;
        Notes = Normalize(dto.Notes);
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void Remove(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    private static string Required(string? value, string message) => string.IsNullOrWhiteSpace(value) ? throw new BadRequestException(message) : value.Trim();
    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public class EmployeeSkill : Entity<Guid>
{
    public Guid EmployeeId { get; private set; }
    public Guid CompanyId { get; private set; }
    public string SkillName { get; private set; } = string.Empty;
    public string? Category { get; private set; }
    public int ProficiencyLevel { get; private set; }
    public string? Source { get; private set; }
    public DateTime? VerifiedAt { get; private set; }

    private EmployeeSkill() { }

    public static EmployeeSkill Create(EmployeeSkillDto dto, Employee employee, string createdBy)
    {
        var skill = new EmployeeSkill
        {
            Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
            EmployeeId = employee.Id,
            CompanyId = employee.CompanyId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
        skill.Update(dto, createdBy);
        skill.CreatedAt = DateTime.UtcNow;
        skill.CreatedBy = createdBy;
        return skill;
    }

    public void Update(EmployeeSkillDto dto, string modifiedBy)
    {
        SkillName = Required(dto.SkillName, "Skill name is required.");
        Category = Normalize(dto.Category);
        ProficiencyLevel = Math.Clamp(dto.ProficiencyLevel, 1, 5);
        Source = Normalize(dto.Source);
        VerifiedAt = dto.VerifiedAt;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void Remove(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    private static string Required(string? value, string message) => string.IsNullOrWhiteSpace(value) ? throw new BadRequestException(message) : value.Trim();
    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public class EmployeeCertification : Entity<Guid>
{
    public Guid EmployeeId { get; private set; }
    public Guid CompanyId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Issuer { get; private set; }
    public DateTime? IssuedAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public Guid? DocumentId { get; private set; }
    public DateTime? VerifiedAt { get; private set; }

    private EmployeeCertification() { }

    public static EmployeeCertification Create(EmployeeCertificationDto dto, Employee employee, string createdBy)
    {
        var certification = new EmployeeCertification
        {
            Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
            EmployeeId = employee.Id,
            CompanyId = employee.CompanyId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
        certification.Update(dto, createdBy);
        certification.CreatedAt = DateTime.UtcNow;
        certification.CreatedBy = createdBy;
        return certification;
    }

    public void Update(EmployeeCertificationDto dto, string modifiedBy)
    {
        Name = Required(dto.Name, "Certification name is required.");
        Issuer = Normalize(dto.Issuer);
        IssuedAt = dto.IssuedAt?.Date;
        ExpiresAt = dto.ExpiresAt?.Date;
        DocumentId = dto.DocumentId;
        VerifiedAt = dto.VerifiedAt;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void Remove(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    private static string Required(string? value, string message) => string.IsNullOrWhiteSpace(value) ? throw new BadRequestException(message) : value.Trim();
    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
