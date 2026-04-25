using Mapster;

namespace Catalog.Products.Features.Products.SearchProducts
{
    public record SearchProductsQuery(string term) : IQuery<SearchProductsResult>;
    public record SearchProductsResult(List<ProductDto> ProductList);
    public class SearchProductsHandler(CatalogDbContext dbContext) : IQueryHandler<SearchProductsQuery, SearchProductsResult>
    {
        public async Task<SearchProductsResult> Handle(SearchProductsQuery query, CancellationToken cancellationToken)
        {
            var products = await dbContext.Products
                .AsNoTracking()
                .Where(p => 
                    p.DeletedAt == null && 
                    (p.Name.ToLower().Contains(query.term.Trim().ToLower()) || 
                        p.NameEng.ToLower().Contains(query.term.Trim().ToLower())))
                .ToListAsync();

            return new SearchProductsResult(products.Adapt<List<ProductDto>>());
        }
    }
}
 