using SuppliersModule.Suppliers.Models;

namespace SuppliersModule.Suppliers.Features.Suppliers.CreateSupplier;

public record CreateSupplierCommand(SupplierDto Supplier) : ICommand<CreateSupplierResult>;
public record CreateSupplierResult(Guid Id);

public class CreateSupplierHandler(SupplierDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CreateSupplierCommand, CreateSupplierResult>
{
    public async Task<CreateSupplierResult> Handle(CreateSupplierCommand command, CancellationToken cancellationToken)
    {
        var user = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("User is not authenticated");

        var companyId = command.Supplier.CompanyId
            ?? throw new BadRequestException("CompanyId is required");

        var supplierCode = string.IsNullOrWhiteSpace(command.Supplier.SupplierCode)
            ? await GenerateSupplierCode(companyId, cancellationToken)
            : command.Supplier.SupplierCode.Trim();

        var exists = await dbContext.Suppliers.AnyAsync(s => s.CompanyId == companyId && s.SupplierCode == supplierCode, cancellationToken);
        if (exists)
            throw new BadRequestException($"Supplier code already exists: {supplierCode}");

        var supplier = Supplier.Create(
            command.Supplier.Name,
            command.Supplier.CommercialName,
            supplierCode,
            command.Supplier.SupplierGroupId,
            command.Supplier.Status,
            command.Supplier.Type,
            command.Supplier.PaymentTerm,
            command.Supplier.TaxNumber,
            command.Supplier.CreditLimit,
            command.Supplier.OpeningBalance,
            command.Supplier.Notes,
            companyId,
            user);

        foreach (var address in command.Supplier.Addresses)
        {
            supplier.AddAddress(address.Title, address.AddressLine1, address.AddressLine2, address.Longitude, address.Latitude, address.City, address.State, address.Country, address.PostalCode, address.IsDefaultBilling, user);
        }

        foreach (var contact in command.Supplier.Contacts)
        {
            supplier.AddContact(contact.FullName, contact.JobTitle, contact.Email, contact.PhoneNumber, contact.IsPrimaryContact, user);
        }

        await dbContext.Suppliers.AddAsync(supplier, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateSupplierResult(supplier.Id);
    }

    private async Task<string> GenerateSupplierCode(Guid companyId, CancellationToken cancellationToken)
    {
        var count = await dbContext.Suppliers.IgnoreQueryFilters().LongCountAsync(s => s.CompanyId == companyId, cancellationToken);
        return $"SUP-{count + 1:00000}";
    }
}
