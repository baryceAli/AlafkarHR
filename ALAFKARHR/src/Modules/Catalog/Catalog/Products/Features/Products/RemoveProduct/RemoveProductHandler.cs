namespace Catalog.Products.Features.Products.RemoveProduct;

public record RemoveProductCommand(Guid Id, string DeletedBy):ICommand<RemoveProductResult>;
public record RemoveProductResult(bool IsSuccess);
public class RemoveProductHandler (CatalogDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<RemoveProductCommand, RemoveProductResult>
{
    public async Task<RemoveProductResult> Handle(RemoveProductCommand request, CancellationToken cancellationToken)
    {
        var product=await dbContext.Products.FindAsync([request.Id]);
        
        if(product is null)
            throw new Exception($"Product not found: {request.Id}");

        //string userName = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Unknown";
        var user = httpContextAccessor.HttpContext?.User;
        var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        product.Remove(userId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new RemoveProductResult(true);
    }
}
