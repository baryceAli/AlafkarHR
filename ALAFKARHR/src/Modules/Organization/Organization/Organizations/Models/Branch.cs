using Shared.DDD;


namespace Organization.Organizations.Models;

public class Branch : Entity<Guid>
{
    public string Name { get; private set; }
    public string NameEng { get; private set; }
    public string Location { get; private set; }
    public double Longitude { get; private set; }
    public double Latitude { get; private set; }

    public string Code { get; private set; }
    public string Phone { get; private set; }
    public string Email { get; private set; }
    public bool IsMainBranch { get; private set; }
    public Guid CompanyId { get; private set; } // 🔴 VERY IMPORTANT
    public Company Company { get; private set; }

    private readonly List<Administration> _administrations = new();
    public IReadOnlyCollection<Administration> Administrations => _administrations;
    private Branch() { }


    public void AddAdministration(Administration administration)
    {
        //var createdDepartment = Department.Create(
        //    department.Id,
        //    department.Name,
        //    department.NameEng,
        //    department.BranchId,
        //    department.CreatedBy);
        if (_administrations.Any(b => b.Id == administration.Id))
            throw new Exception("Administration already exists");

        _administrations.Add(administration);
    }
    public void RemoveAdministration(Administration administration)
    {
        _administrations.Remove(administration);
    }

    public static Branch Create(Guid id,
        string name,
        string nameEng,
        string location,
        double longitude,
        double latitude,
        string code,
        string phone,
        string email,
        bool isMainBranch,
        Guid companyId,
        string createdBy)
    {
        return new Branch
        {
            Id = id,
            Name = name,
            NameEng = nameEng,
            Location = location,
            Longitude = longitude,
            Latitude = latitude,
            Code=code,
            Phone = phone,
            Email = email,
            IsMainBranch = isMainBranch,
            CompanyId = companyId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void Update(
        string name,
        string nameEng,
        string location,
        double longitude,
        double latitude,
        string code,
        string phone,
        string email,
        bool isMainBranch,
        string modifiedBy)
    {
        Name = name;
        NameEng = nameEng;
        Location = location;
        Longitude = longitude;
        Latitude = latitude;
        Code= code;
        Phone = phone;
        Email= email;
        IsMainBranch= isMainBranch;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }
    public void Remove(string deletedBy)
    {
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy.Trim();
        IsDeleted = true;
    }
}
