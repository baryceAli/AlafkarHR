using System.ComponentModel.DataAnnotations;

namespace SharedWithUI.Employees.Dtos;

public class ChangePositionDto
{
    [Required(ErrorMessage ="Emoloyee is required")]
    public Guid Id { get; set; }

    [Required(ErrorMessage ="Position is required")]
    public Guid PositionId { get; set; }
}
