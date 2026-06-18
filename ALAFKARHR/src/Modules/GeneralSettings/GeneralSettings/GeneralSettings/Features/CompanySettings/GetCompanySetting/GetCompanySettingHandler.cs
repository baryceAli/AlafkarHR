using GeneralSettings.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts.CQRS;
using SharedWithUI.GeneralSettings.Dtos;

namespace GeneralSettings.GeneralSettings.Features.CompanySettings.GetCompanySetting;

public record GetCompanySettingQuery(Guid CompanyId) : IQuery<GetCompanySettingResult>;
public record GetCompanySettingResult(CompanySettingDto CompanySetting);

public class GetCompanySettingHandler(GeneralSettingsDbContext dbContext)
    : IQueryHandler<GetCompanySettingQuery, GetCompanySettingResult>
{
    private const double KsaDefaultLatitude = 24.7136;
    private const double KsaDefaultLongitude = 46.6753;

    public async Task<GetCompanySettingResult> Handle(GetCompanySettingQuery request, CancellationToken cancellationToken)
    {
        var setting = await dbContext.CompanySettings
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.CompanyId == request.CompanyId, cancellationToken);

        if (setting is not null)
        {
            return new GetCompanySettingResult(setting.Adapt<CompanySettingDto>());
        }

        return new GetCompanySettingResult(new CompanySettingDto
        {
            Id = Guid.Empty,
            CompanyId = request.CompanyId,
            DefaultLocation = "Riyadh, Saudi Arabia",
            DefaultLatitude = KsaDefaultLatitude,
            DefaultLongitude = KsaDefaultLongitude,
            DefaultPosCustomerId = null
        });
    }
}
