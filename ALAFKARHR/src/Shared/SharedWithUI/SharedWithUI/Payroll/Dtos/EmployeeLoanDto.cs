using SharedWithUI.Payroll.Enums;
using System.ComponentModel.DataAnnotations;

namespace SharedWithUI.Payroll.Dtos;

public class EmployeeLoanDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }

    [Required(ErrorMessage = "Employee is required")]
    public Guid EmployeeId { get; set; }

    public EmployeeLoanType Type { get; set; } = EmployeeLoanType.Loan;

    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
    public decimal Amount { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Installment amount must be greater than zero")]
    public decimal InstallmentAmount { get; set; }

    public decimal DeductedAmount { get; set; }
    public decimal RemainingAmount { get; set; }

    [Range(1, 12, ErrorMessage = "Start month must be between 1 and 12")]
    public int StartMonth { get; set; }

    [Range(1, 9999, ErrorMessage = "Start year is required")]
    public int StartYear { get; set; }

    public EmployeeLoanStatus Status { get; set; } = EmployeeLoanStatus.Draft;
    public string? ReferenceNo { get; set; }
    public string? Notes { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? ApprovedBy { get; set; }
}

public class CreateEmployeeLoanDto
{
    public Guid CompanyId { get; set; }

    [Required(ErrorMessage = "Employee is required")]
    public Guid EmployeeId { get; set; }

    public EmployeeLoanType Type { get; set; } = EmployeeLoanType.Loan;

    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
    public decimal Amount { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Installment amount must be greater than zero")]
    public decimal InstallmentAmount { get; set; }

    [Range(1, 12, ErrorMessage = "Start month must be between 1 and 12")]
    public int StartMonth { get; set; }

    [Range(1, 9999, ErrorMessage = "Start year is required")]
    public int StartYear { get; set; }

    public string? ReferenceNo { get; set; }
    public string? Notes { get; set; }
}

public class UpdateEmployeeLoanDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }

    [Required(ErrorMessage = "Employee is required")]
    public Guid EmployeeId { get; set; }

    public EmployeeLoanType Type { get; set; } = EmployeeLoanType.Loan;

    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
    public decimal Amount { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Installment amount must be greater than zero")]
    public decimal InstallmentAmount { get; set; }

    [Range(1, 12, ErrorMessage = "Start month must be between 1 and 12")]
    public int StartMonth { get; set; }

    [Range(1, 9999, ErrorMessage = "Start year is required")]
    public int StartYear { get; set; }

    public string? ReferenceNo { get; set; }
    public string? Notes { get; set; }
}
