using System.ComponentModel.DataAnnotations;

namespace SharedWithUI.GeneralSettings.Dtos;

public static class WebsiteInquiryOptions
{
    public static readonly string[] Types = ["Contact", "Consultation", "Package", "Product"];
    public static readonly string[] Contexts = ["innovation", "initiatives", "digital", "capabilities"];
    public static readonly string[] Sources = ["/", "/services", "/store", "/projects", "/contact"];
}

public class CreateWebsiteInquiryDto
{
    [Required, StringLength(150, MinimumLength = 2)] public string Name { get; set; } = "";
    [Required, EmailAddress, StringLength(254)] public string Email { get; set; } = "";
    [Required, Phone, StringLength(30, MinimumLength = 7)] public string Mobile { get; set; } = "";
    [Required, StringLength(4000, MinimumLength = 10)] public string Message { get; set; } = "";
    public string RequestType { get; set; } = "Contact";
    public string? ContextKey { get; set; }
    public Guid? ProductSkuId { get; set; }
    public string SourcePage { get; set; } = "/contact";
    [StringLength(200)] public string? Website { get; set; }
}
