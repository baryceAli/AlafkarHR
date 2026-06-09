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
    public decimal TotalSalary { get; set; }
    public decimal TotalAllowances { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal NetSalary => TotalSalary + TotalAllowances - TotalDeductions;

    private readonly List<SalaryRunItem> _salaryRunItems= new();
    public IReadOnlyCollection<SalaryRunItem> SalaryRunItems=> _salaryRunItems;

    public void ClearItems()
    {
        _salaryRunItems.Clear();
    }

    public void AddItem(Guid itemId, ComponentType componentType, decimal amount)
    {
        if (itemId == Guid.Empty)
            throw new ArgumentException("Item is required", nameof(itemId));

        _salaryRunItems.Add(SalaryRunItem.Create(Id, itemId, componentType, amount));
    }

}
