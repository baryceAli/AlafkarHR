using SuppliersModule.Suppliers.Models;

namespace SuppliersModule.Suppliers.Features.SupplierGroups.CreateSupplierGroup;

public record CreateSupplierGroupCommand(SupplierGroupDto SupplierGroup) : ICommand<CreateSupplierGroupResult>;
public record CreateSupplierGroupResult(Guid Id);

public class CreateSupplierGroupHandler(SupplierDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CreateSupplierGroupCommand, CreateSupplierGroupResult>
{
    public async Task<CreateSupplierGroupResult> Handle(CreateSupplierGroupCommand command, CancellationToken cancellationToken)
    {
        var user = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("User is not authenticated");

        var companyId = command.SupplierGroup.CompanyId
            ?? throw new BadRequestException("CompanyId is required");

        var duplicateName = await dbContext.SupplierGroups.AnyAsync(g => g.CompanyId == companyId && g.Name == command.SupplierGroup.Name, cancellationToken);
        if (duplicateName)
            throw new BadRequestException($"Supplier group already exists: {command.SupplierGroup.Name}");

        var supplierGroup = SupplierGroup.Create(
            Guid.NewGuid(),
            command.SupplierGroup.Name,
            command.SupplierGroup.Description,
            command.SupplierGroup.DefaultExpenseAccountId,
            command.SupplierGroup.DefaultPaymentTerm,
            companyId,
            user);

        await dbContext.SupplierGroups.AddAsync(supplierGroup, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateSupplierGroupResult(supplierGroup.Id);
    }
}
