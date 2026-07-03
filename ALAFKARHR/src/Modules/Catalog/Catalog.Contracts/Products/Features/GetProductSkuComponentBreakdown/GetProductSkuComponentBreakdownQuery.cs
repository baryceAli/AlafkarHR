using Shared.Contracts.CQRS;
using SharedWithUI.Catalog.Enums;

namespace Catalog.Contracts.Products.Features.GetProductSkuComponentBreakdown;

public record GetProductSkuComponentBreakdownQuery(Guid CompanyId, Guid ProductSkuId, decimal Quantity)
    : IQuery<GetProductSkuComponentBreakdownResult>;

public record GetProductSkuComponentBreakdownResult(
    Guid CompanyId,
    Guid ParentProductId,
    Guid ParentProductSkuId,
    string ParentSkuCode,
    string ParentSkuCodeEng,
    CatalogProductType ParentProductType,
    SkuProductionType ParentProductionType,
    bool ParentProductIsActive,
    bool ParentSkuIsActive,
    decimal Quantity,
    IReadOnlyList<ProductSkuComponentBreakdownRow> Components);

public record ProductSkuComponentBreakdownRow(
    Guid ComponentProductId,
    Guid ComponentProductSkuId,
    string ComponentProductName,
    string ComponentProductNameEng,
    string ComponentSkuName,
    string ComponentSkuNameEng,
    string ComponentSkuCode,
    string ComponentSkuCodeEng,
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
    decimal QuantityPerParent,
    decimal RequiredQuantity);
