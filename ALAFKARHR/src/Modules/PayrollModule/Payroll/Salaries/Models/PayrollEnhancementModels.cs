using Shared.DDD;

namespace Payroll.Salaries.Models;

public class SalaryStructure : Aggregate<Guid>
{
    public Guid CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameEng { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    private readonly List<SalaryStructureLine> _lines = [];
    public IReadOnlyCollection<SalaryStructureLine> Lines => _lines;

    public void SetLines(IEnumerable<SalaryStructureLine> lines)
    {
        _lines.Clear();
        _lines.AddRange(lines);
    }
}

public class SalaryStructureLine : Entity<Guid>
{
    public Guid SalaryStructureId { get; set; }
    public Guid ComponentId { get; set; }
    public ComponentType ComponentType { get; set; }
    public decimal Amount { get; set; }
    public bool IsRecurring { get; set; } = true;
    public int DisplayOrder { get; set; }
}

public class SalaryStructureAssignment : Aggregate<Guid>
{
    public Guid CompanyId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid SalaryStructureId { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public bool IsActive { get; set; } = true;
}

public class PayrollPeriod : Aggregate<Guid>
{
    public Guid CompanyId { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public PayrollPeriodStatus Status { get; set; } = PayrollPeriodStatus.Open;
    public bool IsClosed => Status == PayrollPeriodStatus.Closed;
}

public class PayrollEntry : Aggregate<Guid>
{
    public Guid CompanyId { get; set; }
    public Guid PayrollPeriodId { get; set; }
    public PayrollEntryStatus Status { get; set; } = PayrollEntryStatus.Draft;
    public int EmployeeCount { get; set; }
    public decimal GrossAmount { get; set; }
    public decimal DeductionAmount { get; set; }
    public decimal NetAmount { get; set; }
    public Guid? AccountingJournalEntryId { get; set; }
    public string? AccountingJournalNumber { get; set; }
    public DateTime? AccountingPostedAt { get; set; }
    public string? AccountingPostedBy { get; set; }
    public bool IsPostedToAccounting => AccountingJournalEntryId.HasValue;
}

public class Payslip : Aggregate<Guid>
{
    public Guid CompanyId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid PayrollEntryId { get; set; }
    public Guid PayrollPeriodId { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public PayslipStatus Status { get; set; } = PayslipStatus.Draft;
    public decimal BasicAmount { get; set; }
    public decimal TotalAllowances { get; set; }
    public decimal TotalBenefits { get; set; }
    public decimal TotalInputs { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal TotalLoans { get; set; }
    public decimal GrossAmount { get; set; }
    public decimal NetAmount { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? PaidAt { get; set; }
    public string? PaidBy { get; set; }
    private readonly List<PayslipLine> _lines = [];
    public IReadOnlyCollection<PayslipLine> Lines => _lines;

    public void SetLines(IEnumerable<PayslipLine> lines)
    {
        _lines.Clear();
        _lines.AddRange(lines);
    }
}

public class PayslipLine : Entity<Guid>
{
    public Guid PayslipId { get; set; }
    public Guid? ComponentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameEng { get; set; }
    public PayrollInputType InputType { get; set; }
    public decimal Amount { get; set; }
    public bool IsDeduction { get; set; }
    public string? SourceType { get; set; }
    public Guid? SourceDocumentId { get; set; }
}

public class PayrollInput : Aggregate<Guid>
{
    public Guid CompanyId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid? PayrollPeriodId { get; set; }
    public PayrollInputType InputType { get; set; }
    public decimal Amount { get; set; }
    public string? Notes { get; set; }
    public bool IsProcessed { get; set; }
}

public class SaudiPayrollInfo : Aggregate<Guid>
{
    public Guid CompanyId { get; set; }
    public Guid EmployeeId { get; set; }
    public string? Iban { get; set; }
    public string? BankCode { get; set; }
    public string? BankName { get; set; }
    public string? GosiNumber { get; set; }
    public decimal GosiEmployeePercentage { get; set; }
    public decimal GosiEmployerPercentage { get; set; }
    public decimal EosBasicSalary { get; set; }
    public DateTime? EosServiceStartDate { get; set; }
    public bool IncludeInWps { get; set; } = true;
}

public class WpsBatch : Aggregate<Guid>
{
    public Guid CompanyId { get; set; }
    public Guid PayrollPeriodId { get; set; }
    public Guid? PayrollEntryId { get; set; }
    public string BatchNumber { get; set; } = string.Empty;
    public WpsBatchStatus Status { get; set; } = WpsBatchStatus.Draft;
    public int EmployeeCount { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime? ExportedAt { get; set; }
    public string? ExportedBy { get; set; }
    private readonly List<WpsBatchRow> _rows = [];
    public IReadOnlyCollection<WpsBatchRow> Rows => _rows;

    public void SetRows(IEnumerable<WpsBatchRow> rows)
    {
        _rows.Clear();
        _rows.AddRange(rows);
        EmployeeCount = _rows.Count;
        TotalAmount = _rows.Sum(x => x.NetAmount);
    }
}

public class WpsBatchRow : Entity<Guid>
{
    public Guid WpsBatchId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid PayslipId { get; set; }
    public string Iban { get; set; } = string.Empty;
    public string? BankCode { get; set; }
    public decimal NetAmount { get; set; }
    public string? Remarks { get; set; }
}

public class EosProvisionSnapshot : Aggregate<Guid>
{
    public Guid CompanyId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid PayrollPeriodId { get; set; }
    public DateTime ServiceStartDate { get; set; }
    public DateTime ServiceEndDate { get; set; }
    public decimal GrossPayBasis { get; set; }
    public decimal ProvisionAmount { get; set; }
    public string? Notes { get; set; }
}

public class PayrollImportedWorkEntry : Aggregate<Guid>
{
    public Guid CompanyId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid PayrollPeriodId { get; set; }
    public Guid? SourceWorkEntryId { get; set; }
    public DateTime WorkDate { get; set; }
    public string EntryType { get; set; } = string.Empty;
    public decimal Hours { get; set; }
    public decimal Amount { get; set; }
    public string? Notes { get; set; }
}
