using Shared.DDD;

namespace Payroll.Salaries.Models;

public class SalaryStructure : Aggregate<Guid>
{
    public string Name { get; set; }
    public string NameEng { get; set; }
    public string Code { get; set; }
    public DateTime? ActiveFrom { get; set; }
    public bool IsActive { get; set; }

    private readonly List<SalaryStructureComponent> _salaryStrucutreComponents = new();
    public IReadOnlyCollection<SalaryStructureComponent> SalaryStrucutreComponents => _salaryStrucutreComponents;

    public SalaryStructure() { }


    public static SalaryStructure Create(
        Guid id,
        string name,
        string nameEng,
        string code,
        //DateTime? activeFrom,
        //bool isActive,
        string createdBy)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException("Name is required");
        if (string.IsNullOrWhiteSpace(nameEng)) throw new ArgumentNullException("NameEng is required");
        if (string.IsNullOrWhiteSpace(code)) throw new ArgumentNullException("Code is required");
        return new SalaryStructure
        {
            Id = id,
            Name = name,
            NameEng = nameEng,
            Code = code,
            IsActive = false,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy,
        };
    }

    public void Update(
        string name,
        string nameEng,
        string code,
        
        string modifiedBy)
    {
        Name = name;
        NameEng = nameEng;
        Code = code;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }
    public void Activate(string modifiedBy)
    {
        IsActive= true;
        ActiveFrom = DateTime.UtcNow;
        ModifiedAt= DateTime.UtcNow;
        ModifiedBy= modifiedBy;

    }
    public void Deactivate(string modifiedBy)
    {
        IsActive = false;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;

    }
}