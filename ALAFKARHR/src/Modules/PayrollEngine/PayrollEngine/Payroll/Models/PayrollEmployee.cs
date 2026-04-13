using Shared.DDD;

namespace PayrollEngine.Payroll.Models;


public class PayrollEmployee : Entity<Guid>
{
    public Guid PayrollRunId { get; private set; }

    public Guid EmployeeId { get; private set; }
    public string EmployeeName { get; private set; }
    public string EmployeeNo { get; private set; }

    public Guid PositionId { get; private set; }

    public decimal BasicSalary { get; private set; }

    // 💰 Earnings
    public decimal AllowancesTotal { get; private set; }
    public decimal OvertimeAmount { get; private set; }
    public decimal Bonuses { get; private set; }

    // ➖ Deductions
    public decimal AbsenceDeduction { get; private set; }
    public decimal LateDeduction { get; private set; }
    public decimal OtherDeductions { get; private set; }

    // 🧮 Final
    public decimal GrossSalary { get; private set; }
    public decimal NetSalary { get; private set; }

    // Attendance context
    public int TotalWorkingDays { get; private set; }
    public int PresentDays { get; private set; }
    public int AbsentDays { get; private set; }
    public int LateMinutes { get; private set; }

    private PayrollEmployee() { }

    public static PayrollEmployee Create(
        Guid id,
        Guid payrollRunId,
        Guid employeeId,
        string employeeName,
        string employeeNo,
        Guid positionId,
        decimal basicSalary)
    {
        return new PayrollEmployee
        {
            Id = id,
            PayrollRunId = payrollRunId,
            EmployeeId = employeeId,
            EmployeeName = employeeName,
            EmployeeNo = employeeNo,
            PositionId = positionId,
            BasicSalary = basicSalary
        };
    }

    public void Calculate()
    {
        GrossSalary =
            BasicSalary +
            AllowancesTotal +
            OvertimeAmount +
            Bonuses;

        var totalDeductions =
            AbsenceDeduction +
            LateDeduction +
            OtherDeductions;

        NetSalary = GrossSalary - totalDeductions;
    }
}