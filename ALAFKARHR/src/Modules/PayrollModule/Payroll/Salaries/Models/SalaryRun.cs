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
    public decimal TaxPercentage { get; set; }
    public decimal TaxableAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal InsurancePercentage { get; set; }
    public decimal InsuranceAmount { get; set; }
    public decimal NetSalary => TotalSalary + TotalAllowances - TotalDeductions - TaxAmount;

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

    public void UndoGeneration(string modifiedBy)
    {
        if (Status != SalaryRunStatus.Calculated)
            throw new InvalidOperationException("Only calculated salary runs can be undone");

        ClearItems();
        TotalAllowances = 0;
        TotalDeductions = 0;
        TaxPercentage = 0;
        TaxableAmount = 0;
        TaxAmount = 0;
        InsurancePercentage = 0;
        InsuranceAmount = 0;
        Status = SalaryRunStatus.Draft;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }
}
