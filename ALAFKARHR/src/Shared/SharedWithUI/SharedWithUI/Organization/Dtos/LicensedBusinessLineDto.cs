namespace SharedWithUI.Organization.Dtos;

using SharedWithUI.Organization.Enums;

public class LicensedBusinessLineDto
{
    public Guid BusinessLineId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public BusinessLineActivationPolicy ActivationPolicy { get; set; } = BusinessLineActivationPolicy.SinglePerCompany;
    public int ActivationLimit { get; set; } = 1;
    public int UsedActivations { get; set; }
    public int AvailableActivations => Math.Max(ActivationLimit - UsedActivations, 0);
}
