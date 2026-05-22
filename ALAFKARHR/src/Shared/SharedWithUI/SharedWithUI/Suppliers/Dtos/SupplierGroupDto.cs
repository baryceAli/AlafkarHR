using SharedWithUI.Suppliers.Enums;

namespace SharedWithUI.Suppliers.Dtos;

public class SupplierGroupDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? DefaultExpenseAccountId { get; set; }
    public SupplierPaymentTermType DefaultPaymentTerm { get; set; }
    public Guid? CompanyId { get; set; }
}
