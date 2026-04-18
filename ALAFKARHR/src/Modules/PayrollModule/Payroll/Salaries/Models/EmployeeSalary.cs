namespace Payroll.Salaries.Models;

public class EmployeeSalary
{
    public Guid EmployeeId { get; set; }
    public Guid SalaryStructureId { get; set; }

    //public List<EmployeeSalaryComponentOverride> Overrides { get; set; }

}
