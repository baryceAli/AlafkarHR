using Shared.DDD;

namespace LeaveManagement.Leave.Models;

public class LeaveApplication : Aggregate<Guid>
{
    public Guid CompanyId { get; private set; }
    public Guid EmployeeId { get; private set; }
    public Guid LeaveTypeId { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public decimal TotalDays { get; private set; }
    public LeaveApplicationStatus Status { get; private set; }
    public string? Reason { get; private set; }
    public string? AttachmentPath { get; private set; }
    public string? ApproverUserId { get; private set; }
    public DateTime? ApprovalDateUtc { get; private set; }
    public string? ApproverComment { get; private set; }

    private LeaveApplication() { }

    public static LeaveApplication Create(Guid id, UpsertLeaveApplicationDto dto, decimal totalDays, string? userId)
    {
        Validate(dto, totalDays);
        return new LeaveApplication
        {
            Id = id,
            CompanyId = dto.CompanyId,
            EmployeeId = dto.EmployeeId,
            LeaveTypeId = dto.LeaveTypeId,
            StartDate = UtcDateTime.Normalize(dto.StartDate).Date,
            EndDate = UtcDateTime.Normalize(dto.EndDate).Date,
            TotalDays = totalDays,
            Status = LeaveApplicationStatus.Draft,
            Reason = string.IsNullOrWhiteSpace(dto.Reason) ? null : dto.Reason.Trim(),
            AttachmentPath = string.IsNullOrWhiteSpace(dto.AttachmentPath) ? null : dto.AttachmentPath.Trim(),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };
    }

    public void UpdateDraft(UpsertLeaveApplicationDto dto, decimal totalDays, string? userId)
    {
        if (Status != LeaveApplicationStatus.Draft)
        {
            throw new BadRequestException("Only draft leave applications can be edited.");
        }

        if (CompanyId != dto.CompanyId)
        {
            throw new BadRequestException("Leave application company cannot be changed.");
        }

        Validate(dto, totalDays);
        EmployeeId = dto.EmployeeId;
        LeaveTypeId = dto.LeaveTypeId;
        StartDate = UtcDateTime.Normalize(dto.StartDate).Date;
        EndDate = UtcDateTime.Normalize(dto.EndDate).Date;
        TotalDays = totalDays;
        Reason = string.IsNullOrWhiteSpace(dto.Reason) ? null : dto.Reason.Trim();
        AttachmentPath = string.IsNullOrWhiteSpace(dto.AttachmentPath) ? null : dto.AttachmentPath.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void Submit(string? userId)
    {
        if (Status != LeaveApplicationStatus.Draft)
        {
            throw new BadRequestException("Only draft leave applications can be submitted.");
        }

        Status = LeaveApplicationStatus.Submitted;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void Approve(string userId, string? comment)
    {
        if (Status != LeaveApplicationStatus.Submitted)
        {
            throw new BadRequestException("Only submitted leave applications can be approved.");
        }

        Status = LeaveApplicationStatus.Approved;
        ApproverUserId = userId;
        ApprovalDateUtc = DateTime.UtcNow;
        ApproverComment = comment;
        ModifiedAt = ApprovalDateUtc;
        ModifiedBy = userId;
    }

    public void Reject(string userId, string? comment)
    {
        if (Status != LeaveApplicationStatus.Submitted)
        {
            throw new BadRequestException("Only submitted leave applications can be rejected.");
        }

        Status = LeaveApplicationStatus.Rejected;
        ApproverUserId = userId;
        ApprovalDateUtc = DateTime.UtcNow;
        ApproverComment = comment;
        ModifiedAt = ApprovalDateUtc;
        ModifiedBy = userId;
    }

    public void Cancel(string? userId)
    {
        if (Status is LeaveApplicationStatus.Approved or LeaveApplicationStatus.Cancelled)
        {
            throw new BadRequestException("Approved or cancelled leave applications cannot be cancelled.");
        }

        Status = LeaveApplicationStatus.Cancelled;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    private static void Validate(UpsertLeaveApplicationDto dto, decimal totalDays)
    {
        if (dto.CompanyId == Guid.Empty || dto.EmployeeId == Guid.Empty || dto.LeaveTypeId == Guid.Empty)
        {
            throw new BadRequestException("Company, employee, and leave type are required.");
        }

        if (UtcDateTime.Normalize(dto.EndDate).Date < UtcDateTime.Normalize(dto.StartDate).Date)
        {
            throw new BadRequestException("Leave end date must be on or after start date.");
        }

        if (totalDays <= 0)
        {
            throw new BadRequestException("Leave request must include at least one configured working day.");
        }
    }
}

public class LeaveLedgerEntry : Entity<Guid>
{
    public Guid CompanyId { get; private set; }
    public Guid EmployeeId { get; private set; }
    public Guid LeaveTypeId { get; private set; }
    public Guid? LeavePeriodId { get; private set; }
    public Guid? SourceDocumentId { get; private set; }
    public LeaveLedgerEntryType EntryType { get; private set; }
    public DateTime PostingDate { get; private set; }
    public decimal Days { get; private set; }
    public decimal BalanceAfter { get; private set; }
    public string? Notes { get; private set; }

    private LeaveLedgerEntry() { }

    public static LeaveLedgerEntry Create(
        Guid id,
        Guid companyId,
        Guid employeeId,
        Guid leaveTypeId,
        Guid? leavePeriodId,
        Guid? sourceDocumentId,
        LeaveLedgerEntryType entryType,
        DateTime postingDate,
        decimal days,
        decimal balanceAfter,
        string? notes,
        string? userId)
    {
        if (companyId == Guid.Empty || employeeId == Guid.Empty || leaveTypeId == Guid.Empty)
        {
            throw new BadRequestException("Company, employee, and leave type are required.");
        }

        if (days == 0)
        {
            throw new BadRequestException("Ledger days cannot be zero.");
        }

        return new LeaveLedgerEntry
        {
            Id = id,
            CompanyId = companyId,
            EmployeeId = employeeId,
            LeaveTypeId = leaveTypeId,
            LeavePeriodId = leavePeriodId,
            SourceDocumentId = sourceDocumentId,
            EntryType = entryType,
            PostingDate = UtcDateTime.Normalize(postingDate).Date,
            Days = days,
            BalanceAfter = balanceAfter,
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim(),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };
    }
}

public class LeaveEncashmentRequest : Entity<Guid>
{
    public Guid CompanyId { get; private set; }
    public Guid EmployeeId { get; private set; }
    public Guid LeaveTypeId { get; private set; }
    public decimal Days { get; private set; }
    public decimal Amount { get; private set; }
    public LeaveApplicationStatus Status { get; private set; }

    private LeaveEncashmentRequest() { }

    public static LeaveEncashmentRequest Create(Guid id, CreateLeaveEncashmentDto dto, string? userId)
    {
        if (dto.CompanyId == Guid.Empty || dto.EmployeeId == Guid.Empty || dto.LeaveTypeId == Guid.Empty)
        {
            throw new BadRequestException("Company, employee, and leave type are required.");
        }

        if (dto.Days <= 0 || dto.Amount < 0)
        {
            throw new BadRequestException("Encashment days must be positive and amount cannot be negative.");
        }

        return new LeaveEncashmentRequest
        {
            Id = id,
            CompanyId = dto.CompanyId,
            EmployeeId = dto.EmployeeId,
            LeaveTypeId = dto.LeaveTypeId,
            Days = dto.Days,
            Amount = dto.Amount,
            Status = LeaveApplicationStatus.Approved,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };
    }
}

