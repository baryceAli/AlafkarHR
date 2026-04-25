namespace Catalog.Products.Features.ProductPackages.RemoveProductPackage;

public record RemoveProductPackageCommand(Guid Id) : ICommand<RemoveProductPackageResult>;
public record RemoveProductPackageResult(bool IsSuccess);


public class RemoveProductPackageHandler (CatalogDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<RemoveProductPackageCommand, RemoveProductPackageResult>
{
    public async Task<RemoveProductPackageResult> Handle(RemoveProductPackageCommand request, CancellationToken cancellationToken)
    {
        var package=await dbContext.ProductPackages.FindAsync([request.Id]);

        if (package is null)
            throw new Exception($"ProductPackage not found: {request.Id}");

        //string userName = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Unknown";
        var user = httpContextAccessor.HttpContext?.User;
        var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        package.Remove(userId);
        await dbContext.SaveChangesAsync();
        return new RemoveProductPackageResult(true);
    }
}
