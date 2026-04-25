namespace Catalog.Products.Features.ProductPackages.GetProductPackageById;

public record GetProductPackageByIdQuery(Guid Id):IQuery<GetProductPackageByIdResult>;
public record GetProductPackageByIdResult(ProductPackageDto ProductPackage);
public class GetProductPackageByIdHandler (CatalogDbContext dbContext): IQueryHandler<GetProductPackageByIdQuery, GetProductPackageByIdResult>
{
    public async Task<GetProductPackageByIdResult> Handle(GetProductPackageByIdQuery request, CancellationToken cancellationToken)
    {
        var productPackage =await dbContext.ProductPackages
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id);
            
        if(productPackage is null)
            throw new Exception($"Product package with id {request.Id} not found.");

        return new GetProductPackageByIdResult(productPackage.Adapt<ProductPackageDto>());
    }
}
