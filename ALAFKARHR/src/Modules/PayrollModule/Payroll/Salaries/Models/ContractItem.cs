using Payroll.Salaries.Models.Enums;
using Shared.DDD;

namespace Payroll.Salaries.Models;

public class ContractItem:Entity<Guid>, ISalaryComponentSource
{
    public Guid ContractId { get; private set; }
    public Guid ComponentId { get; private set; }
    public decimal Amount { get; private set; }
    public Guid CompanyId { get; set; }
    
    internal ContractItem(Guid contractId, 
        Guid componentId, 
        decimal amount,
        Guid companyId)
    {
        ContractId = contractId;
        ComponentId = componentId;
        Amount = amount;
        CompanyId = companyId;
    }

    public SalaryComponentResult Calculate(SalaryContext context)
    {
        return new SalaryComponentResult
        {
            ComponentId = ComponentId,
            Amount = Amount,
            ComponentType = ComponentType.Allowance,
            SourceId = Id
        };
    }
}
