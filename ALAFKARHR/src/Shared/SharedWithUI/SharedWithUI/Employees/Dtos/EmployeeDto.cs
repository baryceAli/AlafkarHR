namespace SharedWithUI.Employees.Dtos;

public class EmployeeDto
{
    public Guid Id { get; set; }
    public string EmployeeNo { get; set; }

    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }

    public string FullName => $"{FirstName} {MiddleName} {LastName}";

    public string Email { get; set; }
    public string Phone { get; set; }

    public DateTime DateOfBirth { get; set; }
    public string NationalId { get; set; }

    public DateTime HireDate { get; set; }
    public DateTime? EndDate { get; set; }

    public bool IsActive { get; set; }

    // 🔗 Organization structure
    public Guid CompanyId { get; set; }
    public string CompanyName { get; set; }
    public Guid BranchId { get; set; }
    public string BranchName { get; set; }
    public Guid AdministrationId { get; set; }
    public string AdministrationName { get; set; }
    public Guid DepartmentId { get; set; }
    public string DepartmentName { get; set; }

    //public Company Company { get; private set; }
    //public Branch Branch { get; private set; }
    //public Administration Administration { get; private set; }
    //public Department Department { get; private set; }

    // 🔗 Job info
    public Guid PositionId { get; set; }
    public string PositionName { get; set; }
    //public Position Position { get; private set; }

    public Guid? ManagerId { get; set; }
    public string? ManagerName { get; set; }
    //public Employee? Manager { get; private set; }

    // 🔐 System
    public string Code { get; set; }

}
