namespace EmployeeModule.Employees.Dtos;

public class PositionDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Code { get; set; }

    public decimal BaseSalary { get; set; }

    public Guid CompanyId { get; set; }

}
