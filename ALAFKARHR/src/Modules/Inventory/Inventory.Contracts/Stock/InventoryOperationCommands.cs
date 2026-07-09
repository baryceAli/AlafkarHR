using Shared.Contracts.CQRS;

namespace Inventory.Contracts.Stock;

public record InventoryOperationChainLine(
    Guid ProductId,
    Guid ProductSkuId,
    Guid? ProductPackageId,
    Guid? UnitId,
    Guid BatchId,
    decimal Quantity,
    decimal UnitCost,
    decimal TotalCost,
    Guid CurrencyId,
    string? Notes,
    Guid? SourceDocumentLineId,
    bool ConsumeReservedQuantity = false);

public record EnsureInventoryReceiptOperationChainCommand(
    Guid CompanyId,
    Guid? BranchId,
    Guid WarehouseId,
    string SourceDocumentType,
    Guid SourceDocumentId,
    string SourceDocumentNumber,
    IReadOnlyList<InventoryOperationChainLine> Lines,
    bool MarkCompleted = false,
    bool MarkFirstStepCompleted = false) : ICommand<EnsureInventoryOperationChainResult>;

public record EnsureInventoryDeliveryOperationChainCommand(
    Guid CompanyId,
    Guid? BranchId,
    Guid WarehouseId,
    string SourceDocumentType,
    Guid SourceDocumentId,
    string SourceDocumentNumber,
    IReadOnlyList<InventoryOperationChainLine> Lines,
    bool MarkCompleted = false) : ICommand<EnsureInventoryOperationChainResult>;

public record EnsureInventoryOperationChainResult(IReadOnlyList<Guid> OperationIds);

public record GetWarehouseOperationFlowQuery(Guid CompanyId, Guid WarehouseId) : IQuery<GetWarehouseOperationFlowResult>;
public record GetWarehouseOperationFlowResult(int InboundFlow, int OutboundFlow);
