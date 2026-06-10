namespace SharedWithUI.Payroll.Dtos;

public class SalaryRunDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid ContractId { get; set; }
    public int SalaryMonth { get; set; }
    public int SalaryYear { get; set; }
    public decimal TotalSalary { get; set; }
    public decimal TotalAllowances { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal TaxPercentage { get; set; }
    public decimal TaxableAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal InsurancePercentage { get; set; }
    public decimal InsuranceAmount { get; set; }
    public decimal NetSalary { get; set; }
    public string Status { get; set; } = string.Empty;
}
