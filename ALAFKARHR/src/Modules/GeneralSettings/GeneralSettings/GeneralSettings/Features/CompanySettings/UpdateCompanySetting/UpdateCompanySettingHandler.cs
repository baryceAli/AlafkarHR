using System.Security.Claims;
using FluentValidation;
using GeneralSettings.Data;
using GeneralSettings.GeneralSettings.Models;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts.CQRS;
using SharedWithUI.GeneralSettings.Dtos;

namespace GeneralSettings.GeneralSettings.Features.CompanySettings.UpdateCompanySetting;

public record UpdateCompanySettingCommand(Guid CompanyId, CompanySettingDto CompanySetting) : ICommand<UpdateCompanySettingResult>;
public record UpdateCompanySettingResult(CompanySettingDto CompanySetting);

public class UpdateCompanySettingCommandValidator : AbstractValidator<UpdateCompanySettingCommand>
{
    public UpdateCompanySettingCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.CompanySetting.DefaultLocation).NotEmpty().MaximumLength(250);
        RuleFor(x => x.CompanySetting.DefaultLatitude).InclusiveBetween(-85, 85);
        RuleFor(x => x.CompanySetting.DefaultLongitude).InclusiveBetween(-180, 180);
    }
}

public class UpdateCompanySettingHandler(GeneralSettingsDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateCompanySettingCommand, UpdateCompanySettingResult>
{
    public async Task<UpdateCompanySettingResult> Handle(UpdateCompanySettingCommand request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?
            .User?
            .FindFirst(ClaimTypes.NameIdentifier)?
            .Value ?? throw new UnauthorizedAccessException("User is not authenticated");

        var setting = await dbContext.CompanySettings
            .FirstOrDefaultAsync(s => s.CompanyId == request.CompanyId, cancellationToken);

        if (setting is null)
        {
            setting = CompanySetting.Create(
                Guid.NewGuid(),
                request.CompanyId,
                request.CompanySetting.DefaultLocation,
                request.CompanySetting.DefaultLatitude,
                request.CompanySetting.DefaultLongitude,
                request.CompanySetting.DefaultPosCustomerId,
                userId);

            await dbContext.CompanySettings.AddAsync(setting, cancellationToken);
        }
        else
        {
            setting.Update(
                request.CompanySetting.DefaultLocation,
                request.CompanySetting.DefaultLatitude,
                request.CompanySetting.DefaultLongitude,
                request.CompanySetting.DefaultPosCustomerId,
                userId);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return new UpdateCompanySettingResult(setting.Adapt<CompanySettingDto>());
    }
}
