using System.ComponentModel.DataAnnotations;

namespace SharedWithUI.Employees.Dtos;

public class TransferDepartmentDto
{
    [Required (ErrorMessage ="Employee is required")]
    public Guid Id { get; set; }

    [Required(ErrorMessage ="Branch is required")]
    public Guid branchId { get; set; }

    public Guid? administrationId { get; set; }

    public Guid? departmentId { get; set; }
}
