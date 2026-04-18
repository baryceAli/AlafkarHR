using Payroll.Salaries.Models.Enums;
using Shared.DDD;

namespace Payroll.Salaries.Models;

public class Component:Entity<Guid>
{
    public string Name { get; private set; }
    public string NameEng { get; private set; }
    public ComponentType ComponentType { get; set; }
    //public bool IsActive { get; private set; }
    public int Order { get; private set; }

}
