using System.ComponentModel.DataAnnotations;

namespace SharedWithUI.Employees.Dtos;

public class SpecializationDto
{
    public Guid Id { get; set; }

    [Required(ErrorMessage ="Name is required")]
    public string Name { get; set; }


    [Required(ErrorMessage ="NameEng is required")]
    public string NameEng { get; set; }
    public Guid CompanyId { get; set; }
}
