using System.ComponentModel.DataAnnotations;

namespace SharedWithUI.Employees.Dtos
{
    public class TerminateEmployeeDto
    {
        [Required(ErrorMessage ="Employoee is required")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Reason is required")]
        [MaxLength(1000, ErrorMessage = "Reason should not exceed 1000 characters")]
        public string reason { get; set; } = default!;
    }
}
