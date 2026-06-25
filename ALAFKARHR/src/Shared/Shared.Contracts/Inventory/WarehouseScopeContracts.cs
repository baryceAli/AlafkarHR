using Shared.Contracts.CQRS;

namespace Shared.Contracts.Inventory;

public record EnsureWarehouseBranchScopeQuery(Guid CompanyId, Guid WarehouseId, Guid BranchId)
    : IQuery<EnsureWarehouseBranchScopeResult>;

public record EnsureWarehouseBranchScopeResult(bool IsValid);
