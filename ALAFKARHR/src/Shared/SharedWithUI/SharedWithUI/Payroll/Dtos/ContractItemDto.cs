using System.ComponentModel.DataAnnotations;

namespace SharedWithUI.Payroll.Dtos;

public class ContractItemDto
{
    [Required(ErrorMessage = "Component ID is required")]
    public Guid ComponentId { get; set; }

    [Required(ErrorMessage = "Value is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Value must be greater than 0")]
    public decimal Value { get; set; }
}
