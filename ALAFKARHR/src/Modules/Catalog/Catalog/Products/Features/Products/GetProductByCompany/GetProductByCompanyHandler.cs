using Catalog.Contracts.Products.Features.GetProductByCompany;

namespace Catalog.Products.Features.Products.GetProductByCompany;


public class GetProductByCompanyHandler(CatalogDbContext dbContext)
    : IQueryHandler<GetProductByCompanyQuery, GetProductByCompanyResult>
{
    public async Task<GetProductByCompanyResult> Handle(
        GetProductByCompanyQuery request,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Products
            .Include(p => p.Skus)
                .ThenInclude(s => s.Variants)
            .Include(p => p.Skus)
                .ThenInclude(s => s.Packages)
                    .ThenInclude(sp => sp.ProductPackage)
            .AsQueryable();

        query = query.Where(p =>
            p.CompanyId == request.companyId &&
            !p.IsDeleted);

        long count = await query.LongCountAsync(cancellationToken);

        var products = await query
            .Skip(request.PaginationRequest.PageSize *
                  request.PaginationRequest.PageIndex)
            .Take(request.PaginationRequest.PageSize)
            .ToListAsync(cancellationToken);

        var categories = await dbContext.Categories
            .Where(c => !c.IsDeleted)
            .ToListAsync(cancellationToken);

        var lastProducts =
            (from product in products
             join c in categories
                 on product.CategoryId equals c.Id
             select new ProductDto
             {
                 Id = product.Id,
                 Name = product.Name,
                 NameEng = product.NameEng,

                 CategoryId = product.CategoryId,
                 CategoryName = c.Name,
                 CategoryNameEng = c.NameEng,

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

                         Name= sku.Name,
                         NameEng = sku.NameEng,
                         SkuCode = sku.SkuCode,
                         SkuCodeEng = sku.SkuCodeEng,
                         Barcode = sku.Barcode,

                         CompanyId = sku.CompanyId,
                         ImageUrl = sku.ImageUrl,
                         SkuKey = sku.SkuKey,
                         UnitId = sku.UnitId,

                         Price = sku.Price,
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
                             .ToList()

                     })
                     .ToList()
             }).ToList();

        return new GetProductByCompanyResult(
            new PaginatedResult<ProductDto>(
                request.PaginationRequest.PageIndex,
                request.PaginationRequest.PageSize,
                count,
                lastProducts));
    }
}

//namespace Catalog.Products.Features.Products.GetProductByCompany;


//public record GetProductByCompanyQuery(Guid companyId, PaginationRequest PaginationRequest) : IQuery<GetProductByCompanyResult>;
//public record GetProductByCompanyResult(PaginatedResult<ProductDto> ProductList);
//public class GetProductByCompanyHandler(CatalogDbContext dbContext)
//    : IQueryHandler<GetProductByCompanyQuery, GetProductByCompanyResult>
//{
//    public async Task<GetProductByCompanyResult> Handle(GetProductByCompanyQuery request, CancellationToken cancellationToken)
//    {
//        var query = dbContext.Products.Include(p => p.Skus).ThenInclude(sku=>sku.Variants).AsQueryable();

//        query = query.Where(p => p.CompanyId == request.companyId && p.IsDeleted == false);
//        long count = await query.LongCountAsync(cancellationToken);



//        var products = await query
//                        .Skip(request.PaginationRequest.PageSize * request.PaginationRequest.PageIndex)
//                        .Take(request.PaginationRequest.PageSize)
//                        .ToListAsync(cancellationToken);
//        var lastProducts = (from product in products
//                            join c in await dbContext.Categories.ToListAsync() on product.CategoryId equals c.Id
//                            //join u in await dbContext.Units.ToListAsync() on product.UnitId equals u.Id
//                            where c.IsDeleted == false //&& u.IsDeleted == false
//                            select new ProductDto
//                            {
//                                Id = product.Id,
//                                Name = product.Name,
//                                NameEng = product.NameEng,
//                                CategoryId = product.CategoryId,
//                                CategoryName = c.Name,
//                                CategoryNameEng = c.NameEng,
//                                //UnitId = product.UnitId,
//                                //UnitName = u.UnitName,
//                                //UnitNameEng = u.UnitNameEng,
//                                CompanyId = product.CompanyId,
//                                Skus = product.Skus
//                                    .Where(sku => sku.IsDeleted == false)
//                                    .Select(sku => new ProductSkuDto
//                                    {
//                                        Id = sku.Id,
//                                        BrandId = sku.BrandId,
//                                        ProductId = sku.ProductId,
//                                        PackageId = sku.PackageId,
//                                        SkuCode = sku.SkuCode,
//                                        SkuCodeEng = sku.SkuCodeEng,
//                                        Barcode = sku.Barcode,
//                                        CompanyId = sku.CompanyId,
//                                        ImageUrl = sku.ImageUrl,
//                                        SkuKey = sku.SkuKey,
//                                        UnitId = sku.UnitId,
//                                        //Variants=sku.Variants,
//                                        Price = sku.Price,
//                                        ShowOnStore = sku.ShowOnStore

//                                        ,
//                                        // ✅ ADD THIS
//                                        Variants = sku.Variants
//                                .Where(v => !v.IsDeleted)
//                                .Select(v => new ProductSkuVariantDto
//                                {
//                                    Id = v.Id,
//                                    ProductSkuId = v.ProductSkuId,
//                                    VariantId = v.VariantId,
//                                    VariantValueId = v.VariantValueId
//                                })
//                                .ToList()

//                                    })
//                        .ToList()




//                            }).ToList()
//                            }).ToList();

//        //var productDto=products.Adapt<List<ProductDto>>();

//        return new GetProductByCompanyResult(
//                        new PaginatedResult<ProductDto>(
//                                        request.PaginationRequest.PageIndex,
//                                        request.PaginationRequest.PageSize,
//                                        count,
//                                        lastProducts));

//    }
//}
