using GeneralSettings.Data;
using GeneralSettings.Data.Seed;
using GeneralSettings.GeneralSettings.Models;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts.CQRS;
using Shared.Contracts.GeneralSettings.Currencies;
using Shared.Exceptions;

namespace GeneralSettings.GeneralSettings.Features.Currencies.Integration;

public class EnsureCompanyInitialCurrenciesHandler(GeneralSettingsDbContext dbContext)
    : ICommandHandler<EnsureCompanyInitialCurrenciesCommand, EnsureCompanyInitialCurrenciesResult>
{
    public async Task<EnsureCompanyInitialCurrenciesResult> Handle(
        EnsureCompanyInitialCurrenciesCommand request,
        CancellationToken cancellationToken)
    {
        var templates = InitialData.currencies
            .Where(currency => !string.IsNullOrWhiteSpace(currency.Code))
            .ToList();

        if (templates.Count == 0)
            throw new BadRequestException("Initial currency templates are not configured.");

        var existingCurrencies = await dbContext.Currencies
            .Where(currency => currency.CompanyId == request.CompanyId && !currency.IsDeleted)
            .ToListAsync(cancellationToken);

        foreach (var template in templates)
        {
            var code = template.Code.Trim().ToUpperInvariant();
            var exists = existingCurrencies.Any(currency =>
                currency.Code.Trim().Equals(code, StringComparison.OrdinalIgnoreCase));

            if (exists)
                continue;

            var currency = Currency.Create(
                Guid.NewGuid(),
                code,
                template.Name.Trim(),
                template.NameEng.Trim(),
                template.Value,
                template.Symbol.Trim(),
                template.IsDefault,
                request.CompanyId,
                request.UserId);

            dbContext.Currencies.Add(currency);
            existingCurrencies.Add(currency);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        var defaultCurrency = existingCurrencies
            .OrderByDescending(currency => currency.IsDefault)
            .ThenBy(currency => currency.Code)
            .FirstOrDefault();

        if (defaultCurrency is null)
            throw new BadRequestException("Unable to resolve a default currency for the company.");

        return new EnsureCompanyInitialCurrenciesResult(defaultCurrency.Id);
    }
}

public class RemoveCompanyCurrenciesHandler(GeneralSettingsDbContext dbContext)
    : ICommandHandler<RemoveCompanyCurrenciesCommand, RemoveCompanyCurrenciesResult>
{
    public async Task<RemoveCompanyCurrenciesResult> Handle(
        RemoveCompanyCurrenciesCommand request,
        CancellationToken cancellationToken)
    {
        var currencies = await dbContext.Currencies
            .Where(currency => currency.CompanyId == request.CompanyId)
            .ToListAsync(cancellationToken);

        dbContext.Currencies.RemoveRange(currencies);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new RemoveCompanyCurrenciesResult(true);
    }
}
