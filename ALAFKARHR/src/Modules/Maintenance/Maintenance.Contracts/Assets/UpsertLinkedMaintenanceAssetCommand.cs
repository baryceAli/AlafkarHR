using Shared.Contracts.CQRS;
using SharedWithUI.Maintenance.Enums;

namespace Maintenance.Contracts.Assets;

public record UpsertLinkedMaintenanceAssetCommand(
    string SourceModule,
    string SourceEntityName,
    Guid SourceEntityId,
    string? AssetCode,
    string Name,
    string NameEng,
    MaintenanceAssetType AssetType,
    MaintenanceAssetStatus Status,
    Guid CompanyId,
    Guid? BranchId,
    Guid? ParentAssetId,
    string? Description,
    string? Location,
    string? SerialNumber,
    DateTime? PurchaseDate,
    DateTime? WarrantyEndDate) : ICommand<UpsertLinkedMaintenanceAssetResult>;

public record UpsertLinkedMaintenanceAssetResult(Guid MaintenanceAssetId);

public record GetLinkedMaintenanceAssetQuery(
    string SourceModule,
    string SourceEntityName,
    Guid SourceEntityId) : IQuery<GetLinkedMaintenanceAssetResult>;

public record GetLinkedMaintenanceAssetResult(Guid? MaintenanceAssetId);
