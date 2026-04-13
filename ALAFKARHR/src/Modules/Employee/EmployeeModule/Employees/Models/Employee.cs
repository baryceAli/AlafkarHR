using Shared.DDD;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace EmployeeModule.Employees.Models;

public class Employee:Aggregate<Guid>
{
    public string EmployeeNo { get; private set; }

    public string FirstName { get; private set; }
    public string LastName { get; private set; }

    public string FullName => $"{FirstName} {LastName}";

    public string Email { get; private set; }
    public string Phone { get; private set; }

    public DateTime DateOfBirth { get; private set; }
    public string NationalId { get; private set; }

    public DateTime HireDate { get; private set; }
    public DateTime? EndDate { get; private set; }

    public bool IsActive { get; private set; }

    // 🔗 Organization structure
    public Guid CompanyId { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid AdministrationId { get; private set; }
    public Guid DepartmentId { get; private set; }

    //public Company Company { get; private set; }
    //public Branch Branch { get; private set; }
    //public Administration Administration { get; private set; }
    //public Department Department { get; private set; }

    // 🔗 Job info
    public Guid PositionId { get; private set; }
    public Position Position { get; private set; }

    public Guid? ManagerId { get; private set; }
    public Employee? Manager { get; private set; }

    // 🔐 System
    public string Code { get; private set; }

    private Employee() { }

    public static Employee Create(
        Guid id,
        string employeeNo,
        string firstName,
        string lastName,
        string email,
        string phone,
        DateTime dateOfBirth,
        string nationalId,
        DateTime hireDate,
        Guid companyId,
        Guid branchId,
        Guid administrationId,
        Guid departmentId,
        Guid positionId,
        string createdBy)
    {
        return new Employee
        {
            Id = id,
            EmployeeNo = employeeNo,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Phone = phone,
            DateOfBirth = dateOfBirth,
            NationalId = nationalId,
            HireDate = hireDate,
            IsActive = true,

            CompanyId = companyId,
            BranchId = branchId,
            AdministrationId = administrationId,
            DepartmentId = departmentId,
            PositionId = positionId,

            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void AssignManager(Guid managerId)
    {
        ManagerId = managerId;
        ModifiedAt = DateTime.UtcNow;
    }

    public void TransferDepartment(Guid branchId, Guid administrationId, Guid departmentId)
    {
        BranchId = branchId;
        AdministrationId = administrationId;
        DepartmentId = departmentId;

        ModifiedAt = DateTime.UtcNow;
    }

    public void Terminate(string reason, string modifiedBy)
    {
        IsActive = false;
        EndDate = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
        ModifiedAt = DateTime.UtcNow;
    }
}
