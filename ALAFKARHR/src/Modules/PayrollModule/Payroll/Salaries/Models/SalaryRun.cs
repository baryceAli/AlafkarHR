using Payroll.Salaries.Models.Enums;
using Shared.DDD;

namespace Payroll.Salaries.Models;

public class SalaryRun:Aggregate<Guid>
{

    public Guid EmployeeId { get; set; }
    public Guid ContractId { get; set; }
    //public DateTime MyProperty { get; set; }
    public int SalaryMonth { get; set; }
    public int SalaryYear { get; set; }
    public SalaryRunStatus Status { get; set; }
    public decimal totalSalary { get; set; }
    public decimal TotalAllowances { get; set; }
    public decimal totalDeductions { get; set; }
    public decimal NetSalary => totalSalary + TotalAllowances - totalDeductions;

    private readonly List<SalaryRunItem> _SalaryRunItems= new();
    public IReadOnlyCollection<SalaryRunItem> SalaryRunItems=> _SalaryRunItems;


}
