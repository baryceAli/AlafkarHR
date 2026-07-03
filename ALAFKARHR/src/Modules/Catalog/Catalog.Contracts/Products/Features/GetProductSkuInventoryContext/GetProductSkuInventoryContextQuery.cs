using Shared.Contracts.CQRS;
using SharedWithUI.Catalog.Enums;

namespace Catalog.Contracts.Products.Features.GetProductSkuInventoryContext;

public record GetProductSkuInventoryContextQuery(Guid CompanyId, Guid ProductSkuId)
    : IQuery<GetProductSkuInventoryContextResult>;

public record GetProductSkuInventoryContextResult(
    Guid CompanyId,
    Guid ProductId,
    Guid ProductSkuId,
    CatalogProductType ProductType,
    SkuProductionType ProductionType,
    bool ProductIsActive,
    bool SkuIsActive,
    bool CategoryIsActive,
    bool BrandIsActive,
    bool UnitIsActive,
    bool IsInventoryTracked,
    Guid UnitId,
    string UnitName,
    string UnitNameEng,
    string UnitCategory,
    decimal UnitConversionFactor,
    IReadOnlyList<GetProductSkuInventoryPackageResult> Packages);

public record GetProductSkuInventoryPackageResult(
    Guid ProductPackageId,
    string Name,
    string NameEng,
    decimal Quantity,
    Guid? UnitId,
    string? UnitName,
    string? UnitNameEng,
    string? UnitCategory,
    decimal? UnitConversionFactor,
    bool UnitIsActive,
    string? Barcode,
    bool IsActive);
