using Shared.DDD;

namespace PayrollEngine.Payroll.Models;

public class PayrollRun : Aggregate<Guid>
{
    public string Code { get; private set; } // e.g. "2026-01"
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }

    public Guid CompanyId { get; private set; }

    public bool IsProcessed { get; private set; }
    public DateTime? ProcessedAt { get; private set; }

    private readonly List<PayrollEmployee> _employees = new();
    public IReadOnlyCollection<PayrollEmployee> Employees => _employees;

    private PayrollRun() { }

    public static PayrollRun Create(Guid id, string code, DateTime start, DateTime end, Guid companyId, string createdBy)
    {
        return new PayrollRun
        {
            Id = id,
            Code = code,
            StartDate = start,
            EndDate = end,
            CompanyId = companyId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void AddEmployee(PayrollEmployee employee)
    {
        if (_employees.Any(x => x.EmployeeId == employee.EmployeeId))
            throw new Exception("Employee already added");

        _employees.Add(employee);
    }

    public void MarkAsProcessed()
    {
        IsProcessed = true;
        ProcessedAt = DateTime.UtcNow;
    }
}