using Catalog.Contracts.Products.Features.GetProductById;

namespace Catalog.Products.Features.Products.GetProductById;

//public record GetProductByIdQuery(Guid Id) : IQuery<GetProductByIdResult>;
//public record GetProductByIdResult(ProductDto Product);

public class GetProductByIdHandler(CatalogDbContext dbContext)
    : IQueryHandler<GetProductByIdQuery, GetProductByIdResult>
{
    public async Task<GetProductByIdResult> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await (
            from p in dbContext.Products
            .Include(x => x.Skus)
            .ThenInclude(s => s.Variants)
            .Include(x => x.Skus)
            .ThenInclude(s => s.Packages)
            .ThenInclude(sp => sp.ProductPackage)
            .Include(x => x.Skus)
            .ThenInclude(s => s.Components)
            .ThenInclude(c => c.ComponentProductSku)
            //.Include(x => x.Packages)

            join c in dbContext.Categories on p.CategoryId equals c.Id
                //join b in dbContext.Brands on p.Skus equals b.Id
                //join u in dbContext.Units on p.UnitId equals u.Id

                where p.Id == request.ProductId 

                select new ProductDto {
                   Id= p.Id,
                   CategoryId= c.Id,
                   CategoryName= c.Name,
                    CategoryNameEng= c.NameEng,
                   //BrandId= b.Id,
                   //BrandName= b.Name,
                    //BrandNameEng=b.NameEng,
                    //UnitId=u.Id,
                    //UnitName=u.UnitName,
                    //UnitNameEng=u.UnitNameEng,
                    Name = p.Name,
                    NameEng = p.NameEng,
                    //Price = p.Price,
                    //ImageUrl = p.ImageUrl,

                    // ✅ FIX HERE
                    Skus = p.Skus
                        .Where(sku => sku.IsDeleted == false)
                        .Select(sku => new ProductSkuDto
                        {
                            Id = sku.Id,
                            BrandId=sku.BrandId,
                            ProductId = sku.ProductId,
                            PackageId = sku.Packages.Select(p => p.ProductPackageId).FirstOrDefault(),
                            PackageName = sku.Packages.Select(p => p.ProductPackage.Name).FirstOrDefault(),
                            PackageNameEng = sku.Packages.Select(p => p.ProductPackage.NameEng).FirstOrDefault(),
                            Name=sku.Name,
                            NameEng=sku.NameEng,
                            SkuCode = sku.SkuCode,
                            SkuCodeEng = sku.SkuCodeEng,
                            Barcode=sku.Barcode,
                            CompanyId=sku.CompanyId,
                            ImageUrl=sku.ImageUrl,
                            SkuKey=sku.SkuKey,
                            UnitId=sku.UnitId,
                            //Variants=sku.Variants,
                            Price = sku.Price,
                            ProductionType = sku.ProductionType,
                            ShowOnStore = sku.ShowOnStore,
                            IsSellable = sku.IsSellable,
                            IsPurchasable = sku.IsPurchasable,
                            IsInventoryTracked = sku.IsInventoryTracked,
                            IsAssetTrackable = sku.IsAssetTrackable,
                            // ✅ ADD THIS
                            Variants = sku.Variants
                                .Where(v => !v.IsDeleted)
                                .Select(v => new ProductSkuVariantDto
                                {
                                    Id= v.Id,
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

                }
            )
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        return new GetProductByIdResult(product);
    }
}
