namespace Catalog.Products.Features.Products.RemoveProductSku;

public record RemoveProductSkuCommand(Guid Id, string DeletedBy) : ICommand<RemoveProductSkuResult>;
public record RemoveProductSkuResult(bool IsSuccess);

public class RemoveProductSkuCommandValidator : AbstractValidator<RemoveProductSkuCommand>
{
    public RemoveProductSkuCommandValidator()
    {
        RuleFor(x => x.DeletedBy).NotEmpty().WithMessage("DeletedBy is required");
    }
}
public class RemoveProductSkuHandler (CatalogDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<RemoveProductSkuCommand, RemoveProductSkuResult>
{
    public async Task<RemoveProductSkuResult> Handle(RemoveProductSkuCommand request, CancellationToken cancellationToken)
    {
        var sku=await dbContext.ProductSkus.FindAsync([request.Id]);
        if (sku is null)
            throw new Exception($"ProductSku not found: {request.Id}");

        //string userName = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Unknown";
        var user = httpContextAccessor.HttpContext?.User;
        var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        sku.Remove(userId);
        await dbContext.SaveChangesAsync();
        return new RemoveProductSkuResult(true);
    }
}
