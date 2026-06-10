using SharedWithUI.Payroll.Enums;
using System.ComponentModel.DataAnnotations;

namespace SharedWithUI.Payroll.Dtos;

public class ComponentDto
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "English name is required")]
    public string NameEng { get; set; } = string.Empty;

    public ComponentType ComponentType { get; set; } = ComponentType.Allowance;
    public bool IsTaxable { get; set; }
    public bool IsActive { get; set; } = true;
    public int Order { get; set; }
    public string? Description { get; set; }
    public Guid CompanyId { get; set; }
}
