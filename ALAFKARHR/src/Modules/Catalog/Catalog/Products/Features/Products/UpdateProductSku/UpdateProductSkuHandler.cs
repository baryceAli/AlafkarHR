using Shared.SaveImages;

namespace Catalog.Products.Features.Products.UpdateProductSku;

public record UpdateProductSkuCommand(ProductSkuDto ProductSku) : ICommand<UpdateProductSkuResult>;
public record UpdateProductSkuResult(bool IsSuccess);

public class UpdateProductSkuCommandValidator : AbstractValidator<UpdateProductSkuCommand>
{
    public UpdateProductSkuCommandValidator()
    {
        RuleFor(x => x.ProductSku.Price).GreaterThan(0).WithMessage("Price must be greator than 0");
        //RuleFor(x => x.ProductSku.VariantValue).NotEmpty().WithMessage("VariantValue is required");
        
    }
}
public class UpdateProductSkuHandler(CatalogDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateProductSkuCommand, UpdateProductSkuResult>
{
    public async Task<UpdateProductSkuResult> Handle(UpdateProductSkuCommand command, CancellationToken cancellationToken)
    {
        var productSku = await dbContext.ProductSkus.Include(sku=> sku.Variants).FirstOrDefaultAsync(sku=>sku.Id==command.ProductSku.Id);
        if (productSku is null)
            throw new Exception($"ProductSku not found: {productSku.Id}");

        //string userName = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Unknown";
        var user = httpContextAccessor.HttpContext?.User;
        var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("User is not authorized");



        string finalImagePath = productSku.ImageUrl;
        var incomingImage = command.ProductSku.ImageUrl;

        if (!string.IsNullOrWhiteSpace(incomingImage))
        {
            if (IsBase64Image(incomingImage))
            {
                string[] PATH_SEGEMNT = ["wwwroot", "Images", "Products"];
                finalImagePath = SaveImages.SaveBase64Image($"{productSku.Id}", PATH_SEGEMNT, command.ProductSku.ImageUrl);
            }
        }

        productSku.Update(
            command.ProductSku.Price, 
            command.ProductSku.ShowOnStore,
            finalImagePath,
            command.ProductSku.Barcode,
            command.ProductSku.Name,
            command.ProductSku.NameEng,
            command.ProductSku.CompanyId,
            command.ProductSku.Variants,
            userId);
        await dbContext.SaveChangesAsync();

        return new UpdateProductSkuResult(true);


    }
    private bool IsBase64Image(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

        if (input.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
            return true;

        Span<byte> buffer = new Span<byte>(new byte[input.Length]);

        return Convert.TryFromBase64String(input, buffer, out _);
    }
}
