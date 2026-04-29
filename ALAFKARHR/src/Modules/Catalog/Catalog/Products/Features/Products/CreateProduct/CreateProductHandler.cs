using Shared.SaveImages;

namespace Catalog.Products.Features.Products.CreateProduct;

public record CreateProductCommand(CreateProductDto Product) : ICommand<CreateProductResult>;
public record CreateProductResult(Guid Id);

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Product.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(x => x.Product.NameEng).NotEmpty().WithMessage("NameEng is required");
        RuleFor(x => x.Product.CategoryId).NotEmpty().WithMessage("Category is required");
        //RuleFor(x => x.Product.BrandId).NotEmpty().WithMessage("Brand is required");
        RuleFor(x => x.Product.UnitId).NotEmpty().WithMessage("Unit is required");
        //RuleFor(x => x.Product.ImageUrl).NotEmpty().WithMessage("ImageFile is required");
        //RuleFor(x => x.Product.Price).GreaterThan(0).WithMessage("Price must be greater than 0");
    }
}
public class CreateProductHandler(CatalogDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CreateProductCommand, CreateProductResult>
{
    public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        //string userName = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Unknown";
        var user = httpContextAccessor.HttpContext?.User;
        var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value??throw new UnauthorizedAccessException("User is not authorized");

        var productId = Guid.NewGuid();
        //string[] PATH_SEGEMNT = ["wwwroot", "Images", "Products"];
        //var img = SaveImages.SaveBase64Image($"{productId}", PATH_SEGEMNT, command.Product.ImageUrl);

        var product = Product.Create(
                productId,
                command.Product.Name,
                command.Product.NameEng,
                command.Product.CategoryId.Value,
                command.Product.UnitId.Value,
                Guid.Parse("4C3D205F-7E2B-42C2-A081-1700B229D91E"),
                
                userId);

        dbContext.Products.Add(product);

        await dbContext.SaveChangesAsync();

        
        //product.Update(product.Name, product.Price, img, userName);
        //product.ImageUrl = img;
        //await dbContext.SaveChangesAsync();
        
        return new CreateProductResult(product.Id);
    }
}
