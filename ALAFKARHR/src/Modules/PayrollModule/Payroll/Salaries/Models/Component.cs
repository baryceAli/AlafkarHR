using Shared.DDD;

namespace Payroll.Salaries.Models;

public class Component:Entity<Guid>
{
    public string Name { get; private set; } = null!;
    public string NameEng { get; private set; } = null!;
    public ComponentType ComponentType { get; private set; }
    public bool IsActive { get; private set; }
    public int Order { get; private set; }
    public string? Description { get; set; }
    public  Guid CompanyId { get; set; }

    private Component() { }

    public static Component Create(
        Guid id,
        string name,
        string nameEng,
        ComponentType componentType,
        int order,
        string? description,
        Guid companyId)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        if (string.IsNullOrWhiteSpace(nameEng)) throw new ArgumentNullException(nameof(nameEng));

        return new Component
        {
            Id = id,
            Name = name,
            NameEng = nameEng,
            ComponentType = componentType,
            IsActive = true,
            Order = order,
            Description = description,
            CompanyId = companyId
        };
    }

}
