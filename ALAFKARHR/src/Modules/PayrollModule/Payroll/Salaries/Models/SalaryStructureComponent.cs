using Payroll.Salaries.Models.Enums;
using Shared.DDD;

namespace Payroll.Salaries.Models;

public class SalaryStructureComponent:Entity<Guid>
{
    public Guid SalaryStructureId { get; private set; }
    public Guid SalaryComponentId { get; private set; }

    public ComponentType Type { get; private set; }

    public decimal? FixedAmount { get; private set; }
    public decimal? Percentage { get; private set; }

    public bool IsTaxable { get; private set; }
    public bool IsActive { get; private set; }

    public int Order { get; private set; }

}
