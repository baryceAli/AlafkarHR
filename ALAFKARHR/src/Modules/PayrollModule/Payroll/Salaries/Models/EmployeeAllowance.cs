using Payroll.Salaries.Models.Enums;
using Shared.DDD;

namespace Payroll.Salaries.Models;

public class EmployeeAllowance:Entity<Guid>
{
    public Guid EmployeeId { get; set; }
    public Guid ComponentId { get; set; }
    public CalculationType CalculationType { get; set; }
    public decimal Value { get; set; }
    public bool IsRecurring { get; set; }
    public bool IsActive { get; set; }

}
