namespace Catalog.Products.Features.Variants.RemoveVariant;

public record RemoveVariantCommand(Guid id) : ICommand<RemoveVariantResult>;
public record RemoveVariantResult(bool IsSuccess);

public class RemoveVariantHandler (CatalogDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<RemoveVariantCommand, RemoveVariantResult>
{
    public async Task<RemoveVariantResult> Handle(RemoveVariantCommand request, CancellationToken cancellationToken)
    {
        var variant = await dbContext.Variants.FindAsync([request.id], cancellationToken);

        if (variant is null)
            throw new Exception($"Variant not found: {request.id}");

        //string userName = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Unknown";
        var user = httpContextAccessor.HttpContext?.User;
        var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value??throw new UnauthorizedAccessException("User is not authorized");

        variant.Remove(userId);

        await dbContext.SaveChangesAsync();

        return new RemoveVariantResult(true);
    }
}
