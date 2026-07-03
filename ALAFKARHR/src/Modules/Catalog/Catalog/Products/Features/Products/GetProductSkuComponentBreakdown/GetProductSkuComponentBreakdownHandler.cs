using Catalog.Contracts.Products.Features.GetProductSkuComponentBreakdown;
using Shared.Exceptions;

namespace Catalog.Products.Features.Products.GetProductSkuComponentBreakdown;

public class GetProductSkuComponentBreakdownHandler(CatalogDbContext dbContext)
    : IQueryHandler<GetProductSkuComponentBreakdownQuery, GetProductSkuComponentBreakdownResult>
{
    public async Task<GetProductSkuComponentBreakdownResult> Handle(
        GetProductSkuComponentBreakdownQuery request,
        CancellationToken cancellationToken)
    {
        if (request.Quantity <= 0)
            throw new BadRequestException("Combo quantity must be greater than zero.");

        var parent = await (
            from sku in dbContext.ProductSkus.AsNoTracking()
            join product in dbContext.Products.AsNoTracking() on sku.ProductId equals product.Id
            where sku.Id == request.ProductSkuId
                  && sku.CompanyId == request.CompanyId
                  && product.CompanyId == request.CompanyId
                  && !sku.IsDeleted
                  && !product.IsDeleted
            select new
            {
                Product = product,
                Sku = sku
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException($"Product SKU not found for company: {request.ProductSkuId}");

        if (parent.Product.ProductType != CatalogProductType.Combo &&
            parent.Sku.ProductionType != SkuProductionType.CompositeBundle)
        {
            throw new BadRequestException($"SKU {parent.Sku.SkuCode} is not a combo/composite bundle.");
        }

        var components = await (
            from component in dbContext.ProductSkuComponents.AsNoTracking()
            join componentSku in dbContext.ProductSkus.AsNoTracking() on component.ComponentProductSkuId equals componentSku.Id
            join componentProduct in dbContext.Products.AsNoTracking() on componentSku.ProductId equals componentProduct.Id
            join componentCategory in dbContext.Categories.AsNoTracking() on componentProduct.CategoryId equals componentCategory.Id
            join componentBrand in dbContext.Brands.AsNoTracking() on componentSku.BrandId equals componentBrand.Id
            join componentUnit in dbContext.Units.AsNoTracking() on componentSku.UnitId equals componentUnit.Id
            where component.ParentProductSkuId == request.ProductSkuId
                  && !component.IsDeleted
                  && componentSku.CompanyId == request.CompanyId
                  && componentProduct.CompanyId == request.CompanyId
                  && !componentSku.IsDeleted
                  && !componentProduct.IsDeleted
                  && !componentCategory.IsDeleted
                  && !componentBrand.IsDeleted
                  && !componentUnit.IsDeleted
            select new ProductSkuComponentBreakdownRow(
                componentProduct.Id,
                componentSku.Id,
                componentProduct.Name,
                componentProduct.NameEng,
                componentSku.Name,
                componentSku.NameEng,
                componentSku.SkuCode,
                componentSku.SkuCodeEng,
                componentProduct.ProductType,
                componentSku.ProductionType,
                componentProduct.IsActive,
                componentSku.IsActive,
                componentCategory.IsActive,
                componentBrand.IsActive,
                componentUnit.IsActive,
                componentSku.IsInventoryTracked,
                componentUnit.Id,
                componentUnit.UnitName,
                componentUnit.UnitNameEng,
                componentUnit.UnitCategory,
                componentUnit.ConversionFactor,
                component.Quantity,
                component.Quantity * request.Quantity))
            .ToListAsync(cancellationToken);

        if (components.Count == 0)
            throw new BadRequestException($"Combo SKU {parent.Sku.SkuCode} has no component SKUs.");

        if (components.Any(component => component.ComponentProductSkuId == request.ProductSkuId))
            throw new BadRequestException($"Combo SKU {parent.Sku.SkuCode} cannot contain itself.");

        if (components.Any(component => component.ProductType == CatalogProductType.Combo ||
                                        component.ProductionType == SkuProductionType.CompositeBundle))
            throw new BadRequestException($"Combo SKU {parent.Sku.SkuCode} contains a nested combo, which is not supported in this tranche.");

        return new GetProductSkuComponentBreakdownResult(
            request.CompanyId,
            parent.Product.Id,
            parent.Sku.Id,
            parent.Sku.SkuCode,
            parent.Sku.SkuCodeEng,
            parent.Product.ProductType,
            parent.Sku.ProductionType,
            parent.Product.IsActive,
            parent.Sku.IsActive,
            request.Quantity,
            components);
    }
}
