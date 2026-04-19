using System.ComponentModel.DataAnnotations;

namespace SharedWithUI.Employees.Dtos;

public class PositionDto
{
    public Guid Id { get; set; }

    
    [Required(ErrorMessage ="Title is required")]
    public string Title { get; set; } = default!;


    [Required(ErrorMessage = "TitleEng is required")]
    public string TitleEng { get; set; } = default!;
    
    
    public string Code { get; set; } = default!;

    [Required(ErrorMessage = "Base salary is required")]
    [Range(1,10000000000,ErrorMessage = "Base salary must be greator than 0")]
    public decimal BaseSalary { get; set; }

    public Guid CompanyId { get; set; }

}
