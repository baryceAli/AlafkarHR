using System.ComponentModel.DataAnnotations;

namespace SharedWithUI.Employees.Dtos;

public class TransferDepartmentDto
{
    [Required (ErrorMessage ="Employee is required")]
    public Guid Id { get; set; }

    [Required(ErrorMessage ="Branch is required")]
    public Guid branchId { get; set; }

    [Required (ErrorMessage ="Administration is required")]
    public Guid administrationId { get; set; }

    [Required (ErrorMessage ="Department is required")]
    public Guid departmentId { get; set; }
}
