using Payroll.Salaries.Models.Enums;

namespace Payroll.Salaries.Models;

public interface ISalaryComponentSource
{
    SalaryComponentResult Calculate(SalaryContext context);
}

public class SalaryComponentResult
{
    public Guid ComponentId { get; set; }
    public string Name { get; set; }

    public decimal Amount { get; set; }

    public ComponentType ComponentType { get; set; } // Allowance / Deduction

    public Guid? SourceId { get; set; } // ContractItemId, AllowanceId, etc.
}

public class SalaryContext
{
    public Guid EmployeeId { get; set; }
    public decimal BasicSalary { get; set; }

    public DateTime Period { get; set; }

    // Add later:
    // public AttendanceData Attendance { get; set; }
    // public OvertimeData Overtime { get; set; }
}