using Shared.Contracts.CQRS;

namespace Shared.Contracts.Inventory;

public record EnsureWarehouseBranchScopeQuery(Guid CompanyId, Guid WarehouseId, Guid BranchId)
    : IQuery<EnsureWarehouseBranchScopeResult>;

public record EnsureWarehouseBranchScopeResult(bool IsValid);

public record EnsureStoreFrontWarehouseCommand(
    Guid CompanyId,
    Guid BranchId,
    Guid? CurrentWarehouseId,
    string Name,
    string NameEng,
    string Code,
    string UserId) : ICommand<EnsureStoreFrontWarehouseResult>;

public record EnsureStoreFrontWarehouseResult(Guid WarehouseId);
