namespace SharedWithUI.Organization.Dtos;

using SharedWithUI.Organization.Enums;

public class BusinessLineDto
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; }
    public BusinessLineActivationPolicy ActivationPolicy { get; set; } = BusinessLineActivationPolicy.SinglePerCompany;
}
