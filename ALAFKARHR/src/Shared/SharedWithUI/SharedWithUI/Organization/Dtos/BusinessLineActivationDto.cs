namespace SharedWithUI.Organization.Dtos;

public class BusinessLineActivationDto
{
    public Guid Id { get; set; }
    public Guid ParentCompanyId { get; set; }
    public Guid CompanyId { get; set; }
    public Guid BusinessLineId { get; set; }
    public string BusinessLineKey { get; set; } = string.Empty;
    public string BusinessLineName { get; set; } = string.Empty;
    public string BusinessLineNameAr { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
