using Catalog.Contracts.Products.Features.GetProductSkuById;

namespace Catalog.Products.Features.Products.GetProductSkuById;

public class GetProductSkuByIdHandler(CatalogDbContext dbContext)
    : IQueryHandler<GetProductSkuByIdQuery, GetProductSkuByIdResult>
{
    public async Task<GetProductSkuByIdResult> Handle(GetProductSkuByIdQuery request, CancellationToken cancellationToken)
    {
        var productSku = await dbContext.ProductSkus
            .Include(s => s.Packages)
            .ThenInclude(p => p.ProductPackage)
            .Include(s => s.Components)
            .ThenInclude(c => c.ComponentProductSku)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (productSku is null)
            throw new Exception($"Product Sku not found: {request.Id}");

        var productSkuDto = productSku.Adapt<ProductSkuDto>();
        var unit = await dbContext.Units.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == productSku.UnitId, cancellationToken);
        productSkuDto.UnitName = unit?.UnitName;
        productSkuDto.UnitNameEng = unit?.UnitNameEng;
        productSkuDto.Packages = productSku.Packages
            .Where(p => !p.IsDeleted && !p.ProductPackage.IsDeleted)
            .Select(p => new ProductPackageDto
            {
                Id = p.ProductPackage.Id,
                Name = p.ProductPackage.Name,
                NameEng = p.ProductPackage.NameEng,
                Quantity = p.ProductPackage.Quantity,
                UnitId = p.ProductPackage.UnitId,
                Barcode = p.ProductPackage.Barcode,
                Weight = p.ProductPackage.Weight,
                Length = p.ProductPackage.Length,
                Width = p.ProductPackage.Width,
                Height = p.ProductPackage.Height,
                Notes = p.ProductPackage.Notes,
                IsActive = p.ProductPackage.IsActive,
                CompanyId = p.ProductPackage.CompanyId
            })
            .ToList();
        productSkuDto.PackageId = productSkuDto.Packages.Select(p => p.Id).FirstOrDefault();
        productSkuDto.PackageName = productSkuDto.Packages.Select(p => p.Name).FirstOrDefault();
        productSkuDto.PackageNameEng = productSkuDto.Packages.Select(p => p.NameEng).FirstOrDefault();
        productSkuDto.Components = productSku.Components
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
            .ToList();

        return new GetProductSkuByIdResult(productSkuDto);
    }
}
