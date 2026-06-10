using System.ComponentModel.DataAnnotations;

namespace SharedWithUI.Payroll.Dtos;

public class EmployeeContractDto
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Employee is required")]
    public Guid EmployeeId { get; set; }

    [Required(ErrorMessage = "Contract is required")]
    public Guid ContractId { get; set; }

    [Required(ErrorMessage = "Company is required")]
    public Guid CompanyId { get; set; }

    public DateTime EffectiveFrom { get; set; } = DateTime.Today;
    public bool IsActive { get; set; } = true;
}
