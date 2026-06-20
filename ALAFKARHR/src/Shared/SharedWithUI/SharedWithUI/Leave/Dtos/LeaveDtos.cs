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
