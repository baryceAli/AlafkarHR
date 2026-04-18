namespace PayrollEngine.Payroll.Models;


public class PayrollCalculator
{
    public void Calculate(PayrollEmployee emp)
    {
        emp.Calculate();
    }

    public decimal CalculateOvertime(decimal hourlyRate, int hours)
    {
        return hourlyRate * hours * 1.5m;
    }

    public decimal CalculateAbsenceDeduction(decimal dailySalary, int absentDays)
    {
        return dailySalary * absentDays;
    }
}