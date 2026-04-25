namespace Catalog.Products.Features.Products.UpdateProductSku;

public record UpdateProductSkuCommand(ProductSkuDto ProductSku) : ICommand<UpdateProductSkuResult>;
public record UpdateProductSkuResult(bool IsSuccess);

public class UpdateProductSkuCommandValidator : AbstractValidator<UpdateProductSkuCommand>
{
    public UpdateProductSkuCommandValidator()
    {
        RuleFor(x => x.ProductSku.Price).GreaterThan(0).WithMessage("Price must be greator than 0");
        RuleFor(x => x.ProductSku.VariantValue).NotEmpty().WithMessage("VariantValue is required");
        
    }
}
public class UpdateProductSkuHandler(CatalogDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateProductSkuCommand, UpdateProductSkuResult>
{
    public async Task<UpdateProductSkuResult> Handle(UpdateProductSkuCommand command, CancellationToken cancellationToken)
    {
        var productSku = await dbContext.ProductSkus.FindAsync([command.ProductSku.Id]);
        if (productSku is null)
            throw new Exception($"ProductSku not found: {productSku.Id}");

        //string userName = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Unknown";
        var user = httpContextAccessor.HttpContext?.User;
        var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        productSku.Update(command.ProductSku.VariantValue, command.ProductSku.Price,command.ProductSku.ShowOnStore, userId);
        await dbContext.SaveChangesAsync();

        return new UpdateProductSkuResult(true);


    }
}
