namespace SuppliersModule.Suppliers.Features.Suppliers.RemoveSupplier;

public record RemoveSupplierCommand(Guid Id) : ICommand<RemoveSupplierResult>;
public record RemoveSupplierResult(bool IsSuccess);

public class RemoveSupplierHandler(SupplierDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<RemoveSupplierCommand, RemoveSupplierResult>
{
    public async Task<RemoveSupplierResult> Handle(RemoveSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = await dbContext.Suppliers.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        if (supplier is null)
            throw new NotFoundException($"Supplier not found: {request.Id}");

        var user = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("User is not authenticated");

        supplier.Remove(user);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new RemoveSupplierResult(true);
    }
}
