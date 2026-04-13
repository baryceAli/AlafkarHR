using PayrollEngine.Payroll.Models.Enums;
using Shared.DDD;

namespace PayrollEngine.Payroll.Models;


public class SalaryComponent : Aggregate<Guid>
{
    public string Name { get; private set; }
    public string Code { get; private set; }

    public ComponentType Type { get; private set; } // Allowance / Deduction / Bonus

    public bool IsPercentage { get; private set; }
    public decimal DefaultValue { get; private set; }

    public Guid CompanyId { get; private set; }
}

