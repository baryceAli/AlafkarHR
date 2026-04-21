using Shared.DDD;

namespace Payroll.Salaries.Models;

public class EmployeeContract:Aggregate<Guid>
{
    public Guid EmployeeId { get; set; }
    public Guid ContractId { get; set; }


    //public List<EmployeeSalaryComponentOverride> Overrides { get; set; }

}
