namespace SuppliersModule.Suppliers.Features.SupplierGroups.RemoveSupplierGroup;

public record RemoveSupplierGroupCommand(Guid Id) : ICommand<RemoveSupplierGroupResult>;
public record RemoveSupplierGroupResult(bool IsSuccess);

public class RemoveSupplierGroupHandler(SupplierDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<RemoveSupplierGroupCommand, RemoveSupplierGroupResult>
{
    public async Task<RemoveSupplierGroupResult> Handle(RemoveSupplierGroupCommand request, CancellationToken cancellationToken)
    {
        var supplierGroup = await dbContext.SupplierGroups.FirstOrDefaultAsync(g => g.Id == request.Id, cancellationToken);
        if (supplierGroup is null)
            throw new NotFoundException($"Supplier group not found: {request.Id}");

        var inUse = await dbContext.Suppliers.AnyAsync(s => s.SupplierGroupId == request.Id, cancellationToken);
        if (inUse)
            throw new BadRequestException("Supplier group is used by one or more suppliers");

        var user = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("User is not authenticated");

        supplierGroup.Remove(user);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new RemoveSupplierGroupResult(true);
    }
}
