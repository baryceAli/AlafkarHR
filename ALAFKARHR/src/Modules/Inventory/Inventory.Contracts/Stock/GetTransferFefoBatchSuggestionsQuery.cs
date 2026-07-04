using Shared.Contracts.CQRS;

namespace Inventory.Contracts.Stock;

public record GetTransferFefoBatchSuggestionsQuery(
    Guid CompanyId,
    Guid SourceWarehouseId,
    Guid ProductSkuId,
    decimal Quantity) : IQuery<GetTransferFefoBatchSuggestionsResult>;

public record GetTransferFefoBatchSuggestionsResult(IReadOnlyList<TransferFefoBatchSuggestionContractDto> Suggestions);

public record TransferFefoBatchSuggestionContractDto(
    Guid BatchId,
    string BatchNumber,
    DateTime? ExpiryDate,
    decimal AvailableQuantity,
    decimal SuggestedQuantity,
    Guid? WarehouseLocationId = null,
    string? LocationCode = null,
    string? LocationName = null,
    string? LocationNameEng = null);
