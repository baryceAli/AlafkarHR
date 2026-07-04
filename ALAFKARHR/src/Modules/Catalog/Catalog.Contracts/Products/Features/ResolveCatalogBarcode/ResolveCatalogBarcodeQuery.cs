using Shared.Contracts.CQRS;
using SharedWithUI.Catalog.Enums;

namespace Catalog.Contracts.Products.Features.ResolveCatalogBarcode;

public record ResolveCatalogBarcodeQuery(Guid CompanyId, string Barcode)
    : IQuery<ResolveCatalogBarcodeResult>;

public record ResolveCatalogBarcodeResult(
    IReadOnlyCollection<ResolvedCatalogBarcodeItem> Items);

public record ResolvedCatalogBarcodeItem(
    Guid CompanyId,
    Guid ProductId,
    Guid? ProductSkuId,
    Guid? ProductPackageId,
    string Code,
    string Name,
    string NameEng,
    CatalogProductType ProductType,
    SkuProductionType? ProductionType,
    bool ProductIsActive,
    bool SkuIsActive,
    bool PackageIsActive,
    bool CategoryIsActive,
    bool BrandIsActive,
    bool UnitIsActive,
    bool IsInventoryTracked,
    decimal PackageQuantity,
    Guid? PackageUnitId,
    decimal? PackageUnitConversionFactor);
