using Shared.Contracts.CQRS;

namespace Catalog.Contracts.Products.Features.GetProductSkuSelectionContext;

public record GetProductSkuSelectionContextQuery(Guid ProductSkuId, Guid CompanyId)
    : IQuery<GetProductSkuSelectionContextResult>;

public record GetProductSkuSelectionContextResult(
    Guid ProductSkuId,
    Guid ProductId,
    Guid? ProductPackageId,
    string Name,
    string? NameEng,
    string SkuCode,
    string? SkuCodeEng,
    Guid UnitId,
    string UnitName,
    string? UnitNameEng,
    decimal? Calories);
