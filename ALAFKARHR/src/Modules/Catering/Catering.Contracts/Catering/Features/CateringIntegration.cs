using Shared.Contracts.CQRS;
using SharedWithUI.Catering.Dtos;

namespace Catering.Contracts.Catering.Features;

public record GetActiveCateringContractsQuery(Guid CompanyId, Guid? CustomerId)
    : IQuery<GetActiveCateringContractsResult>;

public record GetActiveCateringContractsResult(IReadOnlyCollection<CateringContractDto> Contracts);
