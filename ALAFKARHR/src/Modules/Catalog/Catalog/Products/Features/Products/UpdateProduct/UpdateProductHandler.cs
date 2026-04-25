using Shared.SaveImages;

namespace Catalog.Products.Features.Products.UpdateProduct;

public record UpdateProductCommand(ProductDto Product) : ICommand<UpdateProductResult>;
public record UpdateProductResult(bool IsSuccess);

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Product.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(x => x.Product.NameEng).NotEmpty().WithMessage("NameEng is required");
        RuleFor(x => x.Product.ImageUrl).NotEmpty().WithMessage("Image is required");
        RuleFor(x => x.Product.Price).GreaterThan(0).WithMessage("Price must be greator than 0");

    }
}
public class UpdateProductHandler(CatalogDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateProductCommand, UpdateProductResult>
{
    public async Task<UpdateProductResult> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        var product = await dbContext.Products.FindAsync([command.Product.Id], cancellationToken);
        if (product is null)
            throw new Exception($"Product not found: {command.Product.Id}");

        //string userName = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Unknown";
        var user = httpContextAccessor.HttpContext?.User;
        var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        string finalImagePath = product.ImageUrl;
        var incomingImage = command.Product.ImageUrl;

        if (!string.IsNullOrWhiteSpace(incomingImage))
        {
            if (IsBase64Image(incomingImage))
            {
                string[] PATH_SEGEMNT = ["wwwroot", "Images", "Products"];
                finalImagePath = SaveImages.SaveBase64Image($"{product.Id}", PATH_SEGEMNT, command.Product.ImageUrl);
            }
        }
                    

        


        product.Update(command.Product.Name, command.Product.NameEng, command.Product.Price, finalImagePath, userId);

        await dbContext.SaveChangesAsync();

        return new UpdateProductResult(true);
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
