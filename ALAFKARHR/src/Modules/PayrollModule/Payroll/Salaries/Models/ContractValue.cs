using Shared.DDD;

namespace Payroll.Salaries.Models;

public class ContractValue:Entity<Guid>
{
    public Guid ContractId { get; set; }
    public Guid ComponentId { get; set; }
    public decimal Amount { get; set; }

}
