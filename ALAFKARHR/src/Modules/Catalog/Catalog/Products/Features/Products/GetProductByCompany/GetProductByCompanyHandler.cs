using Catalog.Contracts.Products.Features.GetProductByCompany;
using MediatR;
using Pricing.Contracts.Pricings.Features.ResolvePrices;

namespace Catalog.Products.Features.Products.GetProductByCompany;

public class GetProductByCompanyHandler(CatalogDbContext dbContext, ISender sender)
    : IQueryHandler<GetProductByCompanyQuery, GetProductByCompanyResult>,
      IQueryHandler<GetPricedProductByCompanyQuery, GetProductByCompanyResult>
{
    public async Task<GetProductByCompanyResult> Handle(
        GetProductByCompanyQuery request,
        CancellationToken cancellationToken)
        => await GetProductsAsync(request.companyId, null, null, request.PaginationRequest, cancellationToken);

    public async Task<GetProductByCompanyResult> Handle(
        GetPricedProductByCompanyQuery request,
        CancellationToken cancellationToken)
        => await GetProductsAsync(request.companyId, request.CustomerId, request.PriceListId, request.PaginationRequest, cancellationToken);

    private async Task<GetProductByCompanyResult> GetProductsAsync(
        Guid companyId,
        Guid? customerId,
        Guid? priceListId,
        PaginationRequest paginationRequest,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Products
            .Include(p => p.Skus)
                .ThenInclude(s => s.Variants)
            .Include(p => p.Skus)
                .ThenInclude(s => s.Packages)
                    .ThenInclude(sp => sp.ProductPackage)
            .Include(p => p.Skus)
                .ThenInclude(s => s.Components)
                    .ThenInclude(c => c.ComponentProductSku)
            .Where(p => p.CompanyId == companyId && !p.IsDeleted);

        var count = await query.LongCountAsync(cancellationToken);

        var products = await query
            .Skip(paginationRequest.PageSize * paginationRequest.PageIndex)
            .Take(paginationRequest.PageSize)
            .ToListAsync(cancellationToken);

        var categories = await dbContext.Categories
            .Where(c => !c.IsDeleted)
            .ToListAsync(cancellationToken);

        var categoryById = categories.ToDictionary(category => category.Id);
        var productDtos = products.Select(product =>
        {
            categoryById.TryGetValue(product.CategoryId, out var category);

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                NameEng = product.NameEng,
                CategoryId = product.CategoryId,
                CategoryName = category?.Name,
                CategoryNameEng = category?.NameEng,
                CompanyId = product.CompanyId,
                Skus = product.Skus
                    .Where(sku => !sku.IsDeleted)
                    .Select(sku => new ProductSkuDto
                    {
                        Id = sku.Id,
                        BrandId = sku.BrandId,
                        ProductId = sku.ProductId,
                        PackageId = sku.Packages.Select(p => p.ProductPackageId).FirstOrDefault(),
                        PackageName = sku.Packages.Select(p => p.ProductPackage.Name).FirstOrDefault(),
                        PackageNameEng = sku.Packages.Select(p => p.ProductPackage.NameEng).FirstOrDefault(),
                        Name = sku.Name,
                        NameEng = sku.NameEng,
                        SkuCode = sku.SkuCode,
                        SkuCodeEng = sku.SkuCodeEng,
                        Barcode = sku.Barcode,
                        CompanyId = sku.CompanyId,
                        ImageUrl = sku.ImageUrl,
                        SkuKey = sku.SkuKey,
                        UnitId = sku.UnitId,
                        Price = sku.Price,
                        BasePrice = sku.Price,
                        PriceSource = "Catalog",
                        FinalUnitAmount = sku.Price,
                        ProductionType = sku.ProductionType,
                        ShowOnStore = sku.ShowOnStore,
                        Variants = sku.Variants
                            .Where(v => !v.IsDeleted)
                            .Select(v => new ProductSkuVariantDto
                            {
                                Id = v.Id,
                                ProductSkuId = v.ProductSkuId,
                                VariantId = v.VariantId,
                                VariantValueId = v.VariantValueId
                            })
                            .ToList(),
                        Packages = sku.Packages
                            .Where(p => !p.IsDeleted && !p.ProductPackage.IsDeleted)
                            .Select(p => new ProductPackageDto
                            {
                                Id = p.ProductPackage.Id,
                                Name = p.ProductPackage.Name,
                                NameEng = p.ProductPackage.NameEng,
                                Quantity = p.ProductPackage.Quantity,
                                CompanyId = p.ProductPackage.CompanyId
                            })
                            .ToList(),
                        Components = sku.Components
                            .Where(c => !c.IsDeleted && !c.ComponentProductSku.IsDeleted)
                            .Select(c => new ProductSkuComponentDto
                            {
                                Id = c.Id,
                                ParentProductSkuId = c.ParentProductSkuId,
                                ComponentProductSkuId = c.ComponentProductSkuId,
                                ComponentSkuName = c.ComponentProductSku.Name,
                                ComponentSkuNameEng = c.ComponentProductSku.NameEng,
                                ComponentSkuCode = c.ComponentProductSku.SkuCode,
                                ComponentSkuCodeEng = c.ComponentProductSku.SkuCodeEng,
                                Quantity = c.Quantity
                            })
                            .ToList()
                    })
                    .ToList()
            };
        }).ToList();

        if (customerId.HasValue)
        {
            await ApplyResolvedPricesAsync(productDtos, companyId, customerId.Value, priceListId, cancellationToken);
        }

        return new GetProductByCompanyResult(
            new PaginatedResult<ProductDto>(
                paginationRequest.PageIndex,
                paginationRequest.PageSize,
                count,
                productDtos));
    }

    private async Task ApplyResolvedPricesAsync(
        List<ProductDto> products,
        Guid companyId,
        Guid customerId,
        Guid? priceListId,
        CancellationToken cancellationToken)
    {
        var skus = products.SelectMany(product => product.Skus).ToList();
        if (skus.Count == 0)
            return;

        var prices = await sender.Send(
            new ResolvePricesQuery(
                customerId,
                companyId,
                priceListId,
                DateTime.UtcNow,
                skus.Select(sku => new ResolvePriceLineDto(
                    sku.Id,
                    sku.UnitId,
                    1m,
                    sku.TaxRate)).ToList()),
            cancellationToken);

        var priceBySku = prices.Prices.ToDictionary(price => price.ProductSkuId);
        foreach (var sku in skus)
        {
            if (!priceBySku.TryGetValue(sku.Id, out var price))
                continue;

            sku.Price = price.FinalUnitAmount > 0m ? price.FinalUnitAmount : price.UnitPrice;
            sku.PriceListId = price.PriceListId;
            sku.DiscountRate = price.DiscountRate;
            sku.TaxRate = price.TaxRate;
            sku.PriceSource = price.PriceSource;
            sku.PromotionUnitPrice = price.PromotionUnitPrice;
            sku.FinalUnitAmount = price.FinalUnitAmount;
        }
    }
}
