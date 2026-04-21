using Shared.DDD;
using SharedWithUI.Employees.Enums;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace EmployeeModule.Employees.Models;

public class Employee : Aggregate<Guid>
{
    public string EmployeeNo { get; private set; }

    public string FirstName { get; private set; }
    public string FirstNameEng { get; private set; }
    public string MiddleName { get; private set; }
    public string MiddleNameEng { get; private set; }
    public string LastName { get; private set; }
    public string LastNameEng { get; private set; }
    public string? PhotoUrl { get; set; }
    public string FullName => $"{FirstName} {MiddleName} {LastName}";
    public string FullNameEng => $"{FirstNameEng} {MiddleNameEng} {LastNameEng}";

    public string Email { get; private set; }
    public string Phone { get; private set; }

    public DateTime DateOfBirth { get; private set; }
    public string NationalId { get; private set; }

    public DateTime HireDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public string? EndReason { get; private set; }
    public bool IsActive { get; private set; }

    // 🔗 Organization structure
    public Guid CompanyId { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid AdministrationId { get; private set; }
    public Guid? DepartmentId { get; private set; }
    //public Company Company { get; private set; }
    //public Branch Branch { get; private set; }
    //public Administration Administration { get; private set; }
    //public Department Department { get; private set; }

    // 🔗 Job info
    public Guid PositionId { get; private set; }
    public Position Position { get; private set; }

    //public Guid? ManagerId { get; private set; }
    //public Employee? Manager { get; private set; }

    // 🔐 System
    public string Code { get; private set; }

    public EmployeeStatus Status { get; private set; }
    public IdentityType IdentityType { get; private set; }
    public Gender Gender { get; private set; }

    public string Nationality { get; set; }
    public string Address { get; set; }
    public MaritalStatus MaritalStatus { get; set; }
    public EmploymentType EmploymentType { get; set; }
    public Qualification Qualification { get; set; }
    public Guid SpecializationId { get; set; }
    public Guid AcademicInstituteId { get; set; }
    public int GraduationYear { get; set; }
    private Employee() { }

    public static Employee Create(
        Guid id,
        string employeeNo,
        string firstName,
        string firstNameEng,
        string middleName,
        string middleNameEng,
        string lastName,
        string lastNameEng,
        string? photoUrl,
        string email,
        string phone,
        DateTime dateOfBirth,
        string nationalId,
        DateTime hireDate,
        Guid companyId,
        Guid branchId,
        Guid administrationId,
        Guid? departmentId,
        Guid positionId,
        IdentityType identityType,
        Gender gender,
        string code,
        string nationality,
        string address,
        MaritalStatus maritalStatus,
        EmploymentType employmentType,

        Qualification qualification,
        Guid specializationId,
        Guid academicInstituteId,
        int graduationYear,


        string createdBy)
    {


        if (string.IsNullOrEmpty(employeeNo) ||
            string.IsNullOrEmpty(firstName) ||
            string.IsNullOrEmpty(firstNameEng) ||
            string.IsNullOrEmpty(middleName) ||
            string.IsNullOrEmpty(middleNameEng) ||
            string.IsNullOrEmpty(lastName) ||
            string.IsNullOrEmpty(lastNameEng) ||
            string.IsNullOrEmpty(email) ||
            string.IsNullOrEmpty(phone) ||
            string.IsNullOrEmpty(nationalId) ||
            string.IsNullOrEmpty(nationality) ||
            string.IsNullOrEmpty(address)
            //string.IsNullOrEmpty(maritalStatus) ||
            )
        {
            throw new ArgumentNullException("Please make sure you passed all required Data");
        }

        return new Employee
        {
            Id = id,
            EmployeeNo = employeeNo,
            FirstName = firstName,
            FirstNameEng = firstNameEng,
            MiddleName = middleName,
            MiddleNameEng = middleNameEng,
            LastName = lastName,
            LastNameEng = lastNameEng,
            PhotoUrl = photoUrl,
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

            Status = EmployeeStatus.Active,
            Gender = gender,
            IdentityType = identityType,

            Code = code,

            Nationality = nationality,
            Address = address,
            MaritalStatus = maritalStatus,
            EmploymentType = employmentType,
            CreatedAt = DateTime.UtcNow,


            Qualification = qualification,
            SpecializationId = specializationId,
            AcademicInstituteId = academicInstituteId,
            GraduationYear = graduationYear,
            CreatedBy = createdBy
        };
    }

    public void Update(
        string firstName,
        string firstNameEng,
        string middleName,
        string middleNameEng,
        string lastName,
        string lastNameEng,
        string? photoUrl,
        string email,
        string phone,
        string address,
        MaritalStatus maritalStatus,
        EmploymentType employmentType,
        Qualification qualification,
        Guid specializationId,
        Guid academicInstituteId,
        int graduationYear,
        //string nationality,
        string modifiedBy)
    {

        FirstName = firstName;
        FirstNameEng = firstNameEng;
        MiddleName = middleName;
        MiddleNameEng = middleNameEng;
        LastName = lastName;
        LastNameEng = lastNameEng;
        PhotoUrl = photoUrl;
        Email = email;
        Phone = phone;
        Address = address;
        MaritalStatus = maritalStatus;
        EmploymentType = employmentType;
        Qualification= qualification;
        SpecializationId = specializationId;
        AcademicInstituteId= academicInstituteId;
        GraduationYear= graduationYear;
        //Nationality= nationality;
        ModifiedBy = modifiedBy;
        ModifiedAt = DateTime.UtcNow;
    }
    //public void AssignManager(Guid managerId)
    //{
    //    //ManagerId = managerId;
    //    ModifiedAt = DateTime.UtcNow;
    //}

    public void TransferDepartment(Guid branchId, Guid administrationId, Guid? departmentId, string modifiedBy)
    {
        BranchId = branchId;
        AdministrationId = administrationId;
        DepartmentId = departmentId;
        ModifiedBy = modifiedBy;
        ModifiedAt = DateTime.UtcNow;
    }
    public void ChangePosition(Guid positionId, string modifiedBy)
    {
        PositionId = positionId;
        ModifiedBy = modifiedBy;
        ModifiedAt = DateTime.UtcNow;
    }
    public void Terminate(string reason, string modifiedBy)
    {
        if (string.IsNullOrWhiteSpace(reason)) throw new ArgumentNullException("Reason is required");

        IsActive = false;
        EndDate = DateTime.UtcNow;
        EndReason = reason;
        ModifiedBy = modifiedBy;
        ModifiedAt = DateTime.UtcNow;
    }
    public void ChangeStatus(EmployeeStatus status, string modifiedBy)
    {
        Status = status;
        ModifiedBy = modifiedBy;
        ModifiedAt = DateTime.UtcNow;
    }
}
