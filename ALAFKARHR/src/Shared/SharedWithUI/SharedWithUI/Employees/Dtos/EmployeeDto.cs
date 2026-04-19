using SharedWithUI.Employees.Enums;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

namespace SharedWithUI.Employees.Dtos;

public class EmployeeDto
{
    public Guid Id { get; set; }

    [Required(ErrorMessage ="EmployeeNo is required")]
    public string EmployeeNo { get; set; }


    [Required(ErrorMessage ="FirstName is required")]
    public string FirstName { get; set; }

    
    [Required(ErrorMessage = "FirstNameEng is required")]
    public string FirstNameEng { get; set; }

    [Required(ErrorMessage ="MiddleName is required")]
    public string MiddleName { get; set; }


    [Required(ErrorMessage = "MiddleNameEnd is required")]
    public string MiddleNameEng { get; set; }


    [Required(ErrorMessage = "LastName is required")]
    public string LastName { get; set; }

    
    [Required(ErrorMessage = "LastNameEng is required")]
    public string LastNameEng { get; set; }


    public string FullName => $"{FirstName} {MiddleName} {LastName}";



    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; }


    [Required(ErrorMessage = "Phone is required")]
    public string Phone { get; set; }


    [Required(ErrorMessage ="Date of birth is required")]
    public DateTime DateOfBirth { get; set; }


    [Required(ErrorMessage = "NationalId is required")]
    public string NationalId { get; set; }



    [Required(ErrorMessage = "Hire date is required")]
    public DateTime HireDate { get; set; }
    public DateTime? EndDate { get; set; }

    public bool IsActive { get; set; } = true;

    // 🔗 Organization structure

    [Required(ErrorMessage = "Company is required")]
    public Guid? CompanyId { get; set; }
    public string CompanyName { get; set; }


    [Required(ErrorMessage = "Branch is required")]
    public Guid? BranchId { get; set; }
    public string BranchName { get; set; }


    [Required(ErrorMessage = "Administration is required")]
    public Guid? AdministrationId { get; set; }
    public string AdministrationName { get; set; }


    //[Required(ErrorMessage = "Department is required")]
    public Guid? DepartmentId { get; set; }
    public string DepartmentName { get; set; }

    //public Company Company { get; private set; }
    //public Branch Branch { get; private set; }
    //public Administration Administration { get; private set; }
    //public Department Department { get; private set; }

    // 🔗 Job info


    [Required(ErrorMessage = "Position is required")]
    public Guid? PositionId { get; set; }
    public string PositionName { get; set; }
    //public Position Position { get; private set; }

    //public Guid? ManagerId { get; set; }
    //public string? ManagerName { get; set; }
    //public Employee? Manager { get; private set; }

    // 🔐 System
    public string Code { get; set; }

    public IdentityType IdentityType { get; set; }
    public Gender Gender { get; set; }

    [Required(ErrorMessage ="Nationality is required")]
    public string Nationality { get; set; }

    [Required(ErrorMessage ="Address is required")]
    public string Address { get; set; }
    public MaritalStatus MaritalStatus { get; set; }
    public EmploymentType EmploymentType { get; set; }
    public Qualification Qualification { get; set; }
    [Required(ErrorMessage ="Specialization is required")]
    public Guid? SpecializationId { get; set; }

    [Required(ErrorMessage ="Academic Institute is required")]
    public Guid? AcademicInstituteId { get; set; }
    
    [Required(ErrorMessage ="Graduation year is required")]
    [Range(1,100000)]
    public int GraduationYear { get; set; }
}
