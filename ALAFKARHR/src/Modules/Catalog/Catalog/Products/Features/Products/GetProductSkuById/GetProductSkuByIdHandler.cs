namespace Catalog.Products.Features.Products.GetProductSkuById;

public record GetProductSkuByIdQuery(Guid Id):IQuery<GetProductSkuByIdResult>;
public record GetProductSkuByIdResult(ProductSkuDto ProductSku);
public class GetProductSkuByIdHandler (CatalogDbContext dbContext): IQueryHandler<GetProductSkuByIdQuery, GetProductSkuByIdResult>
{
    public async Task<GetProductSkuByIdResult> Handle(GetProductSkuByIdQuery request, CancellationToken cancellationToken)
    {
        var productSku = await dbContext.ProductSkus
            .Include(s => s.Packages)
            .ThenInclude(p => p.ProductPackage)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if(productSku is null)
            throw new Exception($"Product Sku not found: {request.Id}");

        var productSkuDto = productSku.Adapt<ProductSkuDto>();
        productSkuDto.Packages = productSku.Packages
            .Where(p => !p.IsDeleted && !p.ProductPackage.IsDeleted)
            .Select(p => new ProductPackageDto
            {
                Id = p.ProductPackage.Id,
                Name = p.ProductPackage.Name,
                NameEng = p.ProductPackage.NameEng,
                Quantity = p.ProductPackage.Quantity,
                CompanyId = p.ProductPackage.CompanyId
            })
            .ToList();
        productSkuDto.PackageId = productSkuDto.Packages.Select(p => p.Id).FirstOrDefault();
        productSkuDto.PackageName = productSkuDto.Packages.Select(p => p.Name).FirstOrDefault();
        productSkuDto.PackageNameEng = productSkuDto.Packages.Select(p => p.NameEng).FirstOrDefault();

        return new GetProductSkuByIdResult(productSkuDto);
    }
}
