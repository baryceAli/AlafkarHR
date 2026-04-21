using Shared.DDD;

namespace Payroll.Salaries.Models;

public class Contract:Entity<Guid>
{
    public string Name { get; set; }
    public string NameEng { get; set; }
    public string? Description { get; set; }
    public Guid CompanyId { get; set; }

    private readonly List<ContractItem> _Items = new();
    public IReadOnlyCollection<ContractItem> Items=> _Items.AsReadOnly();
    private Contract(){}

    public static Contract Create(Guid id, string name, string nameEng, string? description,Guid companyId,string createdBy)
    {
        if(string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException("Name is required");
        return new Contract
        {
            Id = id,
            Name = name,
            NameEng = nameEng,
            Description = description,
            CompanyId = companyId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }
    public void Update(string name, string nameEng, string? notes, string modifiedBy)
    {
        Name=name;
        NameEng=nameEng;
        Description=notes;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }
    public void Remove(string deletedBy)
    {
        IsDeleted = true;
        DeletedBy=deletedBy;
        DeletedAt = DateTime.UtcNow;
    }

    public void AddContractItem(Guid componentId,decimal amount)
    {
        if (componentId == null || componentId.Equals(Guid.Empty))
            throw new ArgumentNullException("Component is required");

        if (amount <= 0)
            throw new ArgumentOutOfRangeException($"Amount ({amount})  must be greator than 0");


        var existingItem=_Items.FirstOrDefault(i=>i.ComponentId== componentId);
        if(existingItem != null)
            throw new ArgumentException($"Item already exists in the contract: {Id}");
        
        var newItem = new ContractItem(Id,componentId,amount, CompanyId);
        _Items.Add(newItem);
    }

    public void RemoveItem(Guid componentId)
    {
        var existingItem= _Items.FirstOrDefault(i=> i.ComponentId== componentId);

        if(existingItem != null)
            _Items.Remove(existingItem);
    }
}
