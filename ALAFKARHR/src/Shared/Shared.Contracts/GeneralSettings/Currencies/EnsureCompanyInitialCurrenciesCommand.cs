using Shared.Contracts.CQRS;

namespace Shared.Contracts.GeneralSettings.Currencies;

public record EnsureCompanyInitialCurrenciesCommand(Guid CompanyId, string UserId) : ICommand<EnsureCompanyInitialCurrenciesResult>;

public record EnsureCompanyInitialCurrenciesResult(Guid DefaultCurrencyId);
