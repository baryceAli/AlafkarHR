using Shared.DDD;

namespace Payroll.Salaries.Models;

public class EmployeeContract:Aggregate<Guid>
{
    public Guid EmployeeId { get; set; }
    public Guid ContractId { get; set; }
    public Guid CompanyId { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public bool IsActive { get; set; }

    public static EmployeeContract Assign(Guid id, Guid employeeId, Guid contractId, Guid companyId, DateTime effectiveFrom, string createdBy)
    {
        if (employeeId == Guid.Empty) throw new ArgumentException("Employee is required", nameof(employeeId));
        if (contractId == Guid.Empty) throw new ArgumentException("Contract is required", nameof(contractId));
        if (companyId == Guid.Empty) throw new ArgumentException("Company is required", nameof(companyId));

        return new EmployeeContract
        {
            Id = id,
            EmployeeId = employeeId,
            ContractId = contractId,
            CompanyId = companyId,
            EffectiveFrom = effectiveFrom.Date,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void Deactivate(string modifiedBy)
    {
        IsActive = false;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }
}
