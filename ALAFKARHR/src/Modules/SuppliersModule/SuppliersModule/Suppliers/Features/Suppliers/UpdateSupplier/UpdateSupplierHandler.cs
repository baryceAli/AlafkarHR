using SuppliersModule.Suppliers.Models;

namespace SuppliersModule.Suppliers.Features.Suppliers.UpdateSupplier;

public record UpdateSupplierCommand(SupplierDto Supplier) : ICommand<UpdateSupplierResult>;
public record UpdateSupplierResult(bool IsSuccess);

public class UpdateSupplierHandler(SupplierDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateSupplierCommand, UpdateSupplierResult>
{
    public async Task<UpdateSupplierResult> Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = await dbContext.Suppliers
            .Include(s => s.Addresses)
            .Include(s => s.Contacts)
            .FirstOrDefaultAsync(s => s.Id == request.Supplier.Id, cancellationToken);

        if (supplier is null)
            throw new NotFoundException($"Supplier not found: {request.Supplier.Id}");

        var user = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("User is not authenticated");

        var supplierCode = request.Supplier.SupplierCode?.Trim();
        if (string.IsNullOrWhiteSpace(supplierCode))
            throw new BadRequestException("SupplierCode is required");

        var duplicateCode = await dbContext.Suppliers.AnyAsync(s =>
            s.Id != supplier.Id &&
            s.CompanyId == supplier.CompanyId &&
            s.SupplierCode == supplierCode,
            cancellationToken);

        if (duplicateCode)
            throw new BadRequestException($"Supplier code already exists: {supplierCode}");

        supplier.Update(
            request.Supplier.Name,
            request.Supplier.CommercialName,
            supplierCode,
            request.Supplier.SupplierGroupId,
            request.Supplier.Status,
            request.Supplier.Type,
            request.Supplier.PaymentTerm,
            request.Supplier.TaxNumber,
            request.Supplier.CreditLimit,
            request.Supplier.OpeningBalance,
            request.Supplier.Notes,
            request.Supplier.Addresses.Adapt<List<SupplierAddress>>(),
            request.Supplier.Contacts.Adapt<List<SupplierContact>>(),
            user);

        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateSupplierResult(true);
    }
}
