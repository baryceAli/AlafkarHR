using Shared.Contracts.CQRS;
using SharedWithUI.Contracts.Dtos;
using SharedWithUI.Contracts.Enums;

namespace Contracts.Contracts.Contracts.Features;

public record GetPartyContractsQuery(string PartyType, Guid PartyId, Guid CompanyId, ContractStatus? Status)
    : IQuery<GetPartyContractsResult>;

public record GetPartyContractsResult(IReadOnlyCollection<ContractDto> Contracts);

public record GetActiveContractStatusQuery(string PartyType, Guid PartyId, Guid CompanyId, string? ContractType)
    : IQuery<GetActiveContractStatusResult>;

public record GetActiveContractStatusResult(
    bool HasActiveContract,
    Guid? ContractId,
    string? ContractNumber,
    ContractStatus? Status,
    DateTime? EndDate,
    bool HasPendingRenewalPayment);

public record CreateLinkedContractCommand(ContractDto Contract)
    : ICommand<CreateLinkedContractResult>;

public record CreateLinkedContractResult(Guid ContractId, string Number);

public record GetContractRenewalObligationsQuery(Guid CompanyId, DateTime FromDate, DateTime ToDate)
    : IQuery<GetContractRenewalObligationsResult>;

public record GetContractRenewalObligationsResult(IReadOnlyCollection<ContractRenewalObligationDto> Obligations);
