using Shared.DDD;
using SharedWithUI.GeneralSettings.Dtos;

namespace GeneralSettings.GeneralSettings.Models;

public class WebsiteInquiry : Aggregate<Guid>
{
    public Guid CompanyId { get; private set; }
    public string Name { get; private set; } = "";
    public string Email { get; private set; } = "";
    public string Mobile { get; private set; } = "";
    public string Message { get; private set; } = "";
    public string RequestType { get; private set; } = "";
    public string? ContextKey { get; private set; }
    public Guid? ProductSkuId { get; private set; }
    public string SourcePage { get; private set; } = "";
    private WebsiteInquiry() { }
    public static WebsiteInquiry Create(Guid companyId, CreateWebsiteInquiryDto dto) => new()
    {
        Id = Guid.NewGuid(), CompanyId = companyId, Name = dto.Name.Trim(), Email = dto.Email.Trim(),
        Mobile = dto.Mobile.Trim(), Message = dto.Message.Trim(), RequestType = dto.RequestType,
        ContextKey = dto.ContextKey, ProductSkuId = dto.ProductSkuId, SourcePage = dto.SourcePage,
        CreatedAt = DateTime.UtcNow, CreatedBy = "public-website"
    };
}
