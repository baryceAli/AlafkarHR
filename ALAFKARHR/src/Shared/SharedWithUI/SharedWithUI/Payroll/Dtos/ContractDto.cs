using SharedWithUI.Payroll.Enums;
using System.ComponentModel.DataAnnotations;

namespace SharedWithUI.Payroll.Dtos;

public class ContractDto
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }

    [Required(ErrorMessage = "English name is required")]
    public string NameEng { get; set; }

    public string? Description { get; set; }

    [Required(ErrorMessage = "Company ID is required")]
    public Guid CompanyId { get; set; }

    public List<ContractItemDto> Items { get; set; } = new();
}
