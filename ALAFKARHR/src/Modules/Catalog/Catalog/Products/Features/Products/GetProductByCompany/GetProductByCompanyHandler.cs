namespace Catalog.Products.Features.Products.GetProductByCompany;


public record GetProductByCompanyQuery(Guid companyId, PaginationRequest PaginationRequest) : IQuery<GetProductByCompanyResult>;
public record GetProductByCompanyResult(PaginatedResult<ProductDto> ProductList);
public class GetProductByCompanyHandler(CatalogDbContext dbContext)
    : IQueryHandler<GetProductByCompanyQuery, GetProductByCompanyResult>
{
    public async Task<GetProductByCompanyResult> Handle(GetProductByCompanyQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Products.AsQueryable();

        query = query.Where(p => p.CompanyId == request.companyId && p.IsDeleted == false);
        long count =await query.LongCountAsync(cancellationToken);

        

        var products =await query
                        .Skip(request.PaginationRequest.PageSize * request.PaginationRequest.PageIndex)
                        .Take(request.PaginationRequest.PageSize)
                        .ToListAsync(cancellationToken);
        var lastProducts = (from product in products
                            join c in await dbContext.Categories.ToListAsync() on product.CategoryId equals c.Id
                            join u in await dbContext.Units.ToListAsync() on product.UnitId equals u.Id
                            where c.IsDeleted == false && u.IsDeleted == false
                            select new ProductDto
                            {
                                Id = product.Id,
                                Name = product.Name,
                                NameEng = product.NameEng,
                                CategoryId = product.CategoryId,
                                CategoryName = c.Name,
                                CategoryNameEng = c.NameEng,
                                UnitId = product.UnitId,
                                UnitName = u.UnitName,
                                UnitNameEng = u.UnitNameEng,
                                CompanyId = product.CompanyId,
                                Skus = new()
                            }).ToList();

        //var productDto=products.Adapt<List<ProductDto>>();

        return new GetProductByCompanyResult(
                        new PaginatedResult<ProductDto>(
                                        request.PaginationRequest.PageIndex, 
                                        request.PaginationRequest.PageSize, 
                                        count, 
                                        lastProducts));
    
    }
}
