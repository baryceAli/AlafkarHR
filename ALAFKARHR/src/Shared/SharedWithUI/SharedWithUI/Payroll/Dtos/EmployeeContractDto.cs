using System.ComponentModel.DataAnnotations;
using SharedWithUI.Payroll.Enums;

namespace SharedWithUI.Payroll.Dtos;

public class EmployeeContractDto
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Employee is required")]
    public Guid EmployeeId { get; set; }

    [Required(ErrorMessage = "Contract is required")]
    public Guid ContractId { get; set; }

    [Required(ErrorMessage = "Company is required")]
    public Guid CompanyId { get; set; }

    public DateTime EffectiveFrom { get; set; } = DateTime.Today;
    public bool IsActive { get; set; } = true;
}

public enum PayslipStatus
{
    Draft,
    Calculated,
    Approved,
    Paid,
    Closed,
    Cancelled
}

public enum PayrollEntryStatus
{
    Draft,
    Generated,
    Approved,
    Closed,
    Cancelled
}

public enum PayrollPeriodStatus
{
    Open,
    Closed
}

public enum PayrollInputType
{
    Allowance,
    Deduction,
    Benefit,
    Overtime,
    LoanRepayment,
    LeaveDeduction,
    ManualCorrection
}

public enum WpsBatchStatus
{
    Draft,
    Exported,
    Cancelled
}

public class SalaryStructureDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameEng { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public string? StatusLabel { get; set; }
    public List<SalaryStructureLineDto> Lines { get; set; } = [];
}

public class SalaryStructureLineDto
{
    public Guid Id { get; set; }
    public Guid SalaryStructureId { get; set; }
    public Guid ComponentId { get; set; }
    public string? ComponentName { get; set; }
    public string? ComponentNameEng { get; set; }
    public ComponentType ComponentType { get; set; }
    public decimal Amount { get; set; }
    public bool IsRecurring { get; set; } = true;
    public int DisplayOrder { get; set; }
}

public class SalaryStructureAssignmentDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public Guid SalaryStructureId { get; set; }
    public string? SalaryStructureName { get; set; }
    public string? SalaryStructureNameEng { get; set; }
    public DateTime EffectiveFrom { get; set; } = DateTime.Today;
    public DateTime? EffectiveTo { get; set; }
    public bool IsActive { get; set; } = true;
    public string? StatusLabel { get; set; }
}

public class PayrollPeriodDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsClosed { get; set; }
    public PayrollPeriodStatus Status { get; set; }
    public string? PeriodName { get; set; }
    public string? StatusLabel { get; set; }
}

public class PayrollEntryDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid PayrollPeriodId { get; set; }
    public string? PeriodName { get; set; }
    public PayrollEntryStatus Status { get; set; }
    public string? StatusLabel { get; set; }
    public int EmployeeCount { get; set; }
    public decimal GrossAmount { get; set; }
    public decimal DeductionAmount { get; set; }
    public decimal NetAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsPostedToAccounting { get; set; }
    public Guid? AccountingJournalEntryId { get; set; }
    public string? AccountingJournalNumber { get; set; }
    public DateTime? AccountingPostedAt { get; set; }
}

public class PayslipDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public Guid PayrollEntryId { get; set; }
    public Guid PayrollPeriodId { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public PayslipStatus Status { get; set; }
    public string? StatusLabel { get; set; }
    public decimal GrossAmount { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal NetAmount { get; set; }
    public decimal BasicAmount { get; set; }
    public decimal TotalAllowances { get; set; }
    public decimal TotalBenefits { get; set; }
    public decimal TotalInputs { get; set; }
    public decimal TotalLoans { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? PaidAt { get; set; }
    public bool IsWpsEligible { get; set; }
    public List<PayslipLineDto> Lines { get; set; } = [];
}

public class PayslipLineDto
{
    public Guid Id { get; set; }
    public Guid PayslipId { get; set; }
    public Guid? ComponentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameEng { get; set; }
    public PayrollInputType InputType { get; set; }
    public decimal Amount { get; set; }
    public bool IsDeduction { get; set; }
    public string? SourceType { get; set; }
    public Guid? SourceDocumentId { get; set; }
}

public class PayrollInputDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public Guid? PayrollPeriodId { get; set; }
    public string? PeriodName { get; set; }
    public PayrollInputType InputType { get; set; }
    public decimal Amount { get; set; }
    public string? Notes { get; set; }
    public bool IsProcessed { get; set; }
    public string? StatusLabel { get; set; }
}

public class SaudiPayrollInfoDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public string? Iban { get; set; }
    public string? BankCode { get; set; }
    public string? BankName { get; set; }
    public string? GosiNumber { get; set; }
    public decimal GosiEmployeePercentage { get; set; }
    public decimal GosiEmployerPercentage { get; set; }
    public decimal EosBasicSalary { get; set; }
    public DateTime? EosServiceStartDate { get; set; }
    public bool IncludeInWps { get; set; } = true;
    public string? StatusLabel { get; set; }
}

public class SalaryStructureUpsertDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameEng { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<SalaryStructureLineDto> Lines { get; set; } = [];
}

public class SalaryStructureAssignmentUpsertDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid SalaryStructureId { get; set; }
    public DateTime EffectiveFrom { get; set; } = DateTime.Today;
    public DateTime? EffectiveTo { get; set; }
}

public class PayrollPeriodUpsertDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public int Month { get; set; } = DateTime.Today.Month;
    public int Year { get; set; } = DateTime.Today.Year;
    public DateTime StartDate { get; set; } = new(DateTime.Today.Year, DateTime.Today.Month, 1);
    public DateTime EndDate { get; set; } = new(DateTime.Today.Year, DateTime.Today.Month, DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month));
}

public class PayrollEntryCreateDto
{
    public Guid CompanyId { get; set; }
    public Guid PayrollPeriodId { get; set; }
    public List<Guid> EmployeeIds { get; set; } = [];
}

public class PayrollInputUpsertDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid? PayrollPeriodId { get; set; }
    public PayrollInputType InputType { get; set; }
    public decimal Amount { get; set; }
    public string? Notes { get; set; }
}

public class PayrollActionResultDto
{
    public Guid Id { get; set; }
    public bool IsSuccess { get; set; } = true;
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public Guid? ReferenceId { get; set; }
    public string? ReferenceNumber { get; set; }
}

public class SaudiPayrollInfoUpsertDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid EmployeeId { get; set; }
    public string? Iban { get; set; }
    public string? BankCode { get; set; }
    public string? BankName { get; set; }
    public string? GosiNumber { get; set; }
    public decimal GosiEmployeePercentage { get; set; }
    public decimal GosiEmployerPercentage { get; set; }
    public decimal EosBasicSalary { get; set; }
    public DateTime? EosServiceStartDate { get; set; }
    public bool IncludeInWps { get; set; } = true;
}

public class WpsBatchDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid PayrollPeriodId { get; set; }
    public Guid? PayrollEntryId { get; set; }
    public string BatchNumber { get; set; } = string.Empty;
    public WpsBatchStatus Status { get; set; }
    public string? StatusLabel { get; set; }
    public int EmployeeCount { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExportedAt { get; set; }
    public List<WpsBatchRowDto> Rows { get; set; } = [];
}

public class WpsBatchRowDto
{
    public Guid Id { get; set; }
    public Guid WpsBatchId { get; set; }
    public Guid EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public Guid PayslipId { get; set; }
    public string Iban { get; set; } = string.Empty;
    public string? BankCode { get; set; }
    public decimal NetAmount { get; set; }
    public string? Remarks { get; set; }
}

public class CreateWpsBatchDto
{
    public Guid CompanyId { get; set; }
    public Guid PayrollPeriodId { get; set; }
    public Guid? PayrollEntryId { get; set; }
}

public class EosProvisionSnapshotDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public Guid PayrollPeriodId { get; set; }
    public DateTime ServiceStartDate { get; set; }
    public DateTime ServiceEndDate { get; set; }
    public decimal GrossPayBasis { get; set; }
    public decimal ProvisionAmount { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateEosProvisionSnapshotDto
{
    public Guid CompanyId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid PayrollPeriodId { get; set; }
    public DateTime? ServiceEndDate { get; set; }
    public decimal? GrossPayBasis { get; set; }
}

public class PayrollWorkEntryImportDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid PayrollPeriodId { get; set; }
    public Guid? SourceWorkEntryId { get; set; }
    public DateTime WorkDate { get; set; } = DateTime.Today;
    public string EntryType { get; set; } = string.Empty;
    public decimal Hours { get; set; }
    public decimal Amount { get; set; }
    public string? Notes { get; set; }
}
