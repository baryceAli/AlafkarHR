namespace SharedWithUI.Employees.Dtos;

public class PositionDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string TitleEng { get; set; } = default!;
    public string Code { get; set; } = default!;

    public decimal BaseSalary { get; set; }

    public Guid CompanyId { get; set; }

}
