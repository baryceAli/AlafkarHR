using Shared.Contracts.CQRS;

namespace Inventory.Contracts.Stock;

public record GetPutawaySuggestionQuery(
    Guid CompanyId,
    Guid WarehouseId,
    Guid ProductId,
    Guid ProductSkuId) : IQuery<GetPutawaySuggestionResult>;

public record GetPutawaySuggestionResult(PutawaySuggestionContractDto Suggestion);

public record PutawaySuggestionContractDto(
    Guid CompanyId,
    Guid WarehouseId,
    Guid ProductId,
    Guid ProductSkuId,
    Guid? PutawayRuleId,
    Guid? DestinationLocationId,
    string? DestinationLocationCode,
    string? DestinationLocationName,
    string? DestinationLocationNameEng,
    int? Priority,
    string? Warning);
