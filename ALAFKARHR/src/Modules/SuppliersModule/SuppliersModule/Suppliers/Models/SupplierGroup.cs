namespace SuppliersModule.Suppliers.Models;

public class SupplierGroup : Aggregate<Guid>
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public Guid? DefaultExpenseAccountId { get; private set; }
    public SupplierPaymentTermType DefaultPaymentTerm { get; private set; }
    public Guid CompanyId { get; private set; }

    private SupplierGroup() { }

    public static SupplierGroup Create(Guid id, string name, string? description, Guid? defaultExpenseAccountId, SupplierPaymentTermType defaultPaymentTerm, Guid companyId, string createdBy)
    {
        return new SupplierGroup
        {
            Id = id,
            Name = name,
            Description = description,
            DefaultExpenseAccountId = defaultExpenseAccountId,
            DefaultPaymentTerm = defaultPaymentTerm,
            CompanyId = companyId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void Update(string name, string? description, Guid? defaultExpenseAccountId, SupplierPaymentTermType defaultPaymentTerm, string modifiedBy)
    {
        Name = name;
        Description = description;
        DefaultExpenseAccountId = defaultExpenseAccountId;
        DefaultPaymentTerm = defaultPaymentTerm;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void Remove(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }
}
