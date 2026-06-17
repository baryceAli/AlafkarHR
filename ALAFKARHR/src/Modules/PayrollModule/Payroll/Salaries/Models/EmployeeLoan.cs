using Shared.DDD;

namespace Payroll.Salaries.Models;

public class EmployeeLoan : Aggregate<Guid>
{
    public Guid CompanyId { get; private set; }
    public Guid EmployeeId { get; private set; }
    public EmployeeLoanType Type { get; private set; }
    public decimal Amount { get; private set; }
    public decimal InstallmentAmount { get; private set; }
    public decimal DeductedAmount { get; private set; }
    public int StartMonth { get; private set; }
    public int StartYear { get; private set; }
    public EmployeeLoanStatus Status { get; private set; }
    public string? ReferenceNo { get; private set; }
    public string? Notes { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public string? ApprovedBy { get; private set; }

    public decimal RemainingAmount => Math.Max(Amount - DeductedAmount, 0);

    private EmployeeLoan() { }

    public static EmployeeLoan Create(
        Guid companyId,
        Guid employeeId,
        EmployeeLoanType type,
        decimal amount,
        decimal installmentAmount,
        int startMonth,
        int startYear,
        string? referenceNo,
        string? notes,
        string createdBy)
    {
        if (type == EmployeeLoanType.OneTimeDeduction)
        {
            installmentAmount = amount;
        }

        Validate(companyId, employeeId, amount, installmentAmount, startMonth, startYear);

        return new EmployeeLoan
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            EmployeeId = employeeId,
            Type = type,
            Amount = amount,
            InstallmentAmount = installmentAmount,
            DeductedAmount = 0,
            StartMonth = startMonth,
            StartYear = startYear,
            Status = EmployeeLoanStatus.Draft,
            ReferenceNo = referenceNo,
            Notes = notes,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void Update(
        Guid employeeId,
        EmployeeLoanType type,
        decimal amount,
        decimal installmentAmount,
        int startMonth,
        int startYear,
        string? referenceNo,
        string? notes,
        string modifiedBy)
    {
        if (Status != EmployeeLoanStatus.Draft)
            throw new InvalidOperationException("Only draft loans and deductions can be updated");

        if (type == EmployeeLoanType.OneTimeDeduction)
        {
            installmentAmount = amount;
        }

        Validate(CompanyId, employeeId, amount, installmentAmount, startMonth, startYear);

        EmployeeId = employeeId;
        Type = type;
        Amount = amount;
        InstallmentAmount = installmentAmount;
        StartMonth = startMonth;
        StartYear = startYear;
        ReferenceNo = referenceNo;
        Notes = notes;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void Approve(string approvedBy)
    {
        if (Status != EmployeeLoanStatus.Draft)
            throw new InvalidOperationException("Only draft loans and deductions can be approved");

        Status = EmployeeLoanStatus.Approved;
        ApprovedAt = DateTime.UtcNow;
        ApprovedBy = approvedBy;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = approvedBy;
    }

    public void Cancel(string cancelledBy)
    {
        if (Status == EmployeeLoanStatus.Closed)
            throw new InvalidOperationException("Closed loans and deductions cannot be cancelled");

        Status = EmployeeLoanStatus.Cancelled;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = cancelledBy;
    }

    public decimal GetInstallmentForPeriod(int salaryMonth, int salaryYear)
    {
        if (Status != EmployeeLoanStatus.Approved || RemainingAmount <= 0 || !IsStarted(salaryMonth, salaryYear))
            return 0;

        return Math.Min(InstallmentAmount, RemainingAmount);
    }

    public void PostRepayment(decimal amount, string modifiedBy)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount));

        if (Status != EmployeeLoanStatus.Approved)
            throw new InvalidOperationException("Only approved loans and deductions can receive repayments");

        DeductedAmount = Math.Min(DeductedAmount + amount, Amount);
        Status = RemainingAmount <= 0 ? EmployeeLoanStatus.Closed : EmployeeLoanStatus.Approved;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void ReverseRepayment(decimal amount, string modifiedBy)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount));

        DeductedAmount = Math.Max(DeductedAmount - amount, 0);
        if (Status == EmployeeLoanStatus.Closed && RemainingAmount > 0)
        {
            Status = EmployeeLoanStatus.Approved;
        }

        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    private bool IsStarted(int salaryMonth, int salaryYear)
        => salaryYear > StartYear || (salaryYear == StartYear && salaryMonth >= StartMonth);

    private static void Validate(Guid companyId, Guid employeeId, decimal amount, decimal installmentAmount, int startMonth, int startYear)
    {
        if (companyId == Guid.Empty) throw new ArgumentException("Company is required", nameof(companyId));
        if (employeeId == Guid.Empty) throw new ArgumentException("Employee is required", nameof(employeeId));
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
        if (installmentAmount <= 0) throw new ArgumentOutOfRangeException(nameof(installmentAmount));
        if (installmentAmount > amount) throw new ArgumentOutOfRangeException(nameof(installmentAmount), "Installment cannot exceed amount");
        if (startMonth is < 1 or > 12) throw new ArgumentOutOfRangeException(nameof(startMonth));
        if (startYear <= 0) throw new ArgumentOutOfRangeException(nameof(startYear));
    }
}
