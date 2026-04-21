using Payroll.Salaries.Models.Enums;
using Shared.DDD;

namespace Payroll.Salaries.Models;

public class EmployeeDeduction : Entity<Guid>
{
    public Guid EmployeeId { get; set; }
    public Guid ComponentId { get; set; }

    public CalculationType CalculationType { get; set; }

    // Used for Fixed / Percentage
    public decimal Value { get; set; }

    // Used for Installment
    public decimal? TotalAmount { get; set; }
    public decimal? InstallmentAmount { get; set; }
    public decimal? DeductedAmount { get; set; }

    public decimal RemainingAmount =>
        TotalAmount.HasValue ? TotalAmount.Value - (DeductedAmount ?? 0) : 0;

    public bool IsActive { get; set; }
}