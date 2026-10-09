namespace Shared.Contracts.Organization;

public sealed record WebsiteCompanyInfo(Guid Id, string Name, string NameEng);
public interface IWebsiteCompanyReader
{
    Task<List<WebsiteCompanyInfo>> GetActiveCompaniesAsync(Guid parentCompanyId, CancellationToken cancellationToken);
}
