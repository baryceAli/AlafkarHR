using SharedWithUI.Employees.Enums;
using System.ComponentModel.DataAnnotations;

namespace SharedWithUI.Employees.Dtos;

public class CreateEmployeeDto
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "EmployeeNo is required")]
    public string EmployeeNo { get; set; }


    [Required(ErrorMessage = "FirstName is required")]
    public string FirstName { get; set; }


    [Required(ErrorMessage = "MiddleName is required")]
    public string MiddleName { get; set; }


    [Required(ErrorMessage = "LastName is required")]
    public string LastName { get; set; }




    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; }


    [Required(ErrorMessage = "Phone is required")]
    public string Phone { get; set; }


    [Required(ErrorMessage = "Date of birth is required")]
    public DateTime DateOfBirth { get; set; }


    [Required(ErrorMessage = "NationalId is required")]
    public string NationalId { get; set; }



    [Required(ErrorMessage = "Hire date is required")]
    public DateTime HireDate { get; set; }
    
    public DateTime? EndDate { get; set; }


    // 🔗 Organization structure

    [Required(ErrorMessage = "Company is required")]
    public Guid? CompanyId { get; set; }


    [Required(ErrorMessage = "Branch is required")]
    public Guid? BranchId { get; set; }


    [Required(ErrorMessage = "Administration is required")]
    public Guid? AdministrationId { get; set; }


    [Required(ErrorMessage = "Department is required")]
    public Guid? DepartmentId { get; set; }

    // 🔗 Job info


    [Required(ErrorMessage = "Position is required")]
    public Guid? PositionId { get; set; }


    public IdentityType IdentityType { get; set; }
    public Gender Gender { get; set; }

}
