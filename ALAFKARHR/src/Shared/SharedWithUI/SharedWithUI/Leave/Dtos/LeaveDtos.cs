using SharedWithUI.Attendance.Enums;

namespace SharedWithUI.Leave.Dtos;

public class EmergencyLeaveRequestDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid CompanyId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? AttachmentPath { get; set; }
    public AttendanceExceptionStatus Status { get; set; }
    public string? ApproverUserId { get; set; }
    public DateTime? ApprovalDateUtc { get; set; }
    public string? ApproverComment { get; set; }
}

public class CreateEmergencyLeaveRequestDto
{
    public Guid EmployeeId { get; set; }
    public Guid CompanyId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? AttachmentPath { get; set; }
}

public class ReviewEmergencyLeaveRequestDto
{
    public Guid RequestId { get; set; }
    public bool IsApproved { get; set; }
    public string? ApproverComment { get; set; }
}

public class EmployeeLeaveBalanceDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid CompanyId { get; set; }
    public int Year { get; set; } = DateTime.Today.Year;
    public decimal AnnualLeaveDays { get; set; }
    public bool AllowCarryForward { get; set; }
    public decimal MaxCarryForwardDays { get; set; }
    public decimal CarriedForwardDays { get; set; }
    public decimal TakenDays { get; set; }
    public decimal AvailableDays { get; set; }
    public decimal RemainingDays { get; set; }
}

public class UpsertEmployeeLeaveBalanceDto
{
    public Guid? Id { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid CompanyId { get; set; }
    public int Year { get; set; } = DateTime.Today.Year;
    public decimal AnnualLeaveDays { get; set; } = 30;
    public bool AllowCarryForward { get; set; } = true;
    public decimal MaxCarryForwardDays { get; set; } = 5;
}

public class LeaveReportFilterDto
{
    public Guid CompanyId { get; set; }
    public int Year { get; set; } = DateTime.Today.Year;
    public Guid? EmployeeId { get; set; }
    public AttendanceExceptionStatus? Status { get; set; }
}

public class LeaveReportRowDto
{
    public Guid EmployeeId { get; set; }
    public int Year { get; set; }
    public decimal AnnualLeaveDays { get; set; }
    public decimal CarriedForwardDays { get; set; }
    public decimal AvailableDays { get; set; }
    public decimal TakenDays { get; set; }
    public decimal RemainingDays { get; set; }
    public int PendingRequests { get; set; }
    public int ApprovedRequests { get; set; }
    public int RejectedRequests { get; set; }
}

public class LeaveReportDto
{
    public int Year { get; set; }
    public List<LeaveReportRowDto> Rows { get; set; } = [];
}

public enum LeaveApplicationStatus
{
    Draft,
    Submitted,
    Approved,
    Rejected,
    Cancelled
}

public enum LeaveLedgerEntryType
{
    Allocation,
    Application,
    Adjustment,
    CarryForward,
    Encashment,
    Expiry
}

public enum LeavePolicyAssignmentTarget
{
    Company,
    Department,
    Employee
}

public class LeaveTypeDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string NameEng { get; set; } = string.Empty;
    public bool IsPaid { get; set; } = true;
    public bool AllowNegativeBalance { get; set; }
    public decimal NegativeBalanceLimit { get; set; }
    public bool RequiresAttachment { get; set; }
    public bool IsEmergencyLeave { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpsertLeaveTypeDto
{
    public Guid? Id { get; set; }
    public Guid CompanyId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string NameEng { get; set; } = string.Empty;
    public bool IsPaid { get; set; } = true;
    public bool AllowNegativeBalance { get; set; }
    public decimal NegativeBalanceLimit { get; set; }
    public bool RequiresAttachment { get; set; }
    public bool IsEmergencyLeave { get; set; }
    public bool IsActive { get; set; } = true;
}

public class LeavePeriodDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsClosed { get; set; }
}

public class UpsertLeavePeriodDto
{
    public Guid? Id { get; set; }
    public Guid CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; } = new(DateTime.Today.Year, 1, 1);
    public DateTime EndDate { get; set; } = new(DateTime.Today.Year, 12, 31);
    public bool IsClosed { get; set; }
}

public class LeavePolicyDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameEng { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public List<LeavePolicyLineDto> Lines { get; set; } = [];
}

public class UpsertLeavePolicyDto
{
    public Guid? Id { get; set; }
    public Guid CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameEng { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public List<LeavePolicyLineDto> Lines { get; set; } = [];
}

public class LeavePolicyLineDto
{
    public Guid? Id { get; set; }
    public Guid LeaveTypeId { get; set; }
    public string? LeaveTypeName { get; set; }
    public string? LeaveTypeNameEng { get; set; }
    public decimal AnnualAllocationDays { get; set; }
    public bool AccruesMonthly { get; set; }
    public bool AllowCarryForward { get; set; }
    public decimal MaxCarryForwardDays { get; set; }
}

public class LeavePolicyAssignmentDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid PolicyId { get; set; }
    public string? PolicyName { get; set; }
    public string? PolicyNameEng { get; set; }
    public LeavePolicyAssignmentTarget Target { get; set; }
    public Guid? EmployeeId { get; set; }
    public Guid? DepartmentId { get; set; }
    public DateTime EffectiveFrom { get; set; } = DateTime.Today;
    public DateTime? EffectiveTo { get; set; }
}

public class UpsertLeavePolicyAssignmentDto
{
    public Guid? Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid PolicyId { get; set; }
    public LeavePolicyAssignmentTarget Target { get; set; } = LeavePolicyAssignmentTarget.Company;
    public Guid? EmployeeId { get; set; }
    public Guid? DepartmentId { get; set; }
    public DateTime EffectiveFrom { get; set; } = DateTime.Today;
    public DateTime? EffectiveTo { get; set; }
}

public class GenerateLeaveAllocationsDto
{
    public Guid CompanyId { get; set; }
    public Guid LeavePeriodId { get; set; }
    public Guid? EmployeeId { get; set; }
}

public class LeaveApplicationDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid LeaveTypeId { get; set; }
    public string? LeaveTypeName { get; set; }
    public string? LeaveTypeNameEng { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalDays { get; set; }
    public LeaveApplicationStatus Status { get; set; }
    public string? Reason { get; set; }
    public string? AttachmentPath { get; set; }
    public string? ApproverUserId { get; set; }
    public DateTime? ApprovalDateUtc { get; set; }
    public string? ApproverComment { get; set; }
    public DateTime? CreatedAt { get; set; }
}

public class UpsertLeaveApplicationDto
{
    public Guid? Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid LeaveTypeId { get; set; }
    public DateTime StartDate { get; set; } = DateTime.Today;
    public DateTime EndDate { get; set; } = DateTime.Today;
    public string? Reason { get; set; }
    public string? AttachmentPath { get; set; }
}

public class ReviewLeaveApplicationDto
{
    public Guid ApplicationId { get; set; }
    public bool IsApproved { get; set; }
    public string? ApproverComment { get; set; }
}

public class LeaveLedgerEntryDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid LeaveTypeId { get; set; }
    public string? LeaveTypeName { get; set; }
    public string? LeaveTypeNameEng { get; set; }
    public Guid? LeavePeriodId { get; set; }
    public Guid? SourceDocumentId { get; set; }
    public LeaveLedgerEntryType EntryType { get; set; }
    public DateTime PostingDate { get; set; } = DateTime.Today;
    public decimal Days { get; set; }
    public decimal BalanceAfter { get; set; }
    public string? Notes { get; set; }
}

public class LeaveEncashmentDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid LeaveTypeId { get; set; }
    public decimal Days { get; set; }
    public decimal Amount { get; set; }
    public LeaveApplicationStatus Status { get; set; }
}

public class CreateLeaveLedgerAdjustmentDto
{
    public Guid CompanyId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid LeaveTypeId { get; set; }
    public Guid? LeavePeriodId { get; set; }
    public DateTime PostingDate { get; set; } = DateTime.Today;
    public decimal Days { get; set; }
    public string? Notes { get; set; }
}

public class CreateLeaveEncashmentDto
{
    public Guid CompanyId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid LeaveTypeId { get; set; }
    public decimal Days { get; set; }
    public decimal Amount { get; set; }
}
