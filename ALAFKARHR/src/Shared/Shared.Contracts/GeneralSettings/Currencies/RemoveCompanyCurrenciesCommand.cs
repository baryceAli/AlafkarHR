using Shared.Contracts.CQRS;

namespace Shared.Contracts.GeneralSettings.Currencies;

public record RemoveCompanyCurrenciesCommand(Guid CompanyId) : ICommand<RemoveCompanyCurrenciesResult>;

public record RemoveCompanyCurrenciesResult(bool IsSuccess);
