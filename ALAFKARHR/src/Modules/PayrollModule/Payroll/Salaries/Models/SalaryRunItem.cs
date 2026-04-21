using Payroll.Salaries.Models.Enums;
using Shared.DDD;

namespace Payroll.Salaries.Models;

public class SalaryRunItem:Entity<Guid>
{
    public Guid SalaryRunId { get; set; }
    //public Guid EmployeeId { get; set; }
    //public Guid ContractId { get; set; }
    public Guid ItemId { get; set; }
    public ComponentType ComponentType{ get; set; }
    public decimal Amount { get; set; }
}
