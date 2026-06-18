using System.ComponentModel.DataAnnotations;

namespace SharedWithUI.GeneralSettings.Dtos;

public class CompanySettingDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }

    [Required]
    [MaxLength(250)]
    public string DefaultLocation { get; set; } = "Riyadh, Saudi Arabia";

    [Range(-85, 85)]
    public double DefaultLatitude { get; set; } = 24.7136;

    [Range(-180, 180)]
    public double DefaultLongitude { get; set; } = 46.6753;

    public Guid? DefaultPosCustomerId { get; set; }
}
