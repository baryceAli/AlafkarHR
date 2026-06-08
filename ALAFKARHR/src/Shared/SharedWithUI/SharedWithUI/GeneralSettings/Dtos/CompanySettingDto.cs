namespace SharedWithUI.GeneralSettings.Dtos;

public class CompanySettingDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string DefaultLocation { get; set; } = "Riyadh, Saudi Arabia";
    public double DefaultLatitude { get; set; } = 24.7136;
    public double DefaultLongitude { get; set; } = 46.6753;
}
