namespace SharedWithUI.Suppliers.Dtos;

public class SupplierContactDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? JobTitle { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsPrimaryContact { get; set; }
}
