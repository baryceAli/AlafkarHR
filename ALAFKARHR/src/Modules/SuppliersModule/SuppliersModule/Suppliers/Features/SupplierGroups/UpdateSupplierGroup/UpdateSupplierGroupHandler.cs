namespace SuppliersModule.Suppliers.Features.SupplierGroups.UpdateSupplierGroup;

public record UpdateSupplierGroupCommand(SupplierGroupDto SupplierGroup) : ICommand<UpdateSupplierGroupResult>;
public record UpdateSupplierGroupResult(bool IsSuccess);

public class UpdateSupplierGroupHandler(SupplierDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateSupplierGroupCommand, UpdateSupplierGroupResult>
{
    public async Task<UpdateSupplierGroupResult> Handle(UpdateSupplierGroupCommand request, CancellationToken cancellationToken)
    {
        var supplierGroup = await dbContext.SupplierGroups.FirstOrDefaultAsync(g => g.Id == request.SupplierGroup.Id, cancellationToken);
        if (supplierGroup is null)
            throw new NotFoundException($"Supplier group not found: {request.SupplierGroup.Id}");

        var user = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("User is not authenticated");

        var duplicateName = await dbContext.SupplierGroups.AnyAsync(g =>
            g.Id != supplierGroup.Id &&
            g.CompanyId == supplierGroup.CompanyId &&
            g.Name == request.SupplierGroup.Name,
            cancellationToken);

        if (duplicateName)
            throw new BadRequestException($"Supplier group already exists: {request.SupplierGroup.Name}");

        supplierGroup.Update(
            request.SupplierGroup.Name,
            request.SupplierGroup.Description,
            request.SupplierGroup.DefaultExpenseAccountId,
            request.SupplierGroup.DefaultPaymentTerm,
            user);

        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateSupplierGroupResult(true);
    }
}
