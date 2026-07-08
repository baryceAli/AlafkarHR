using SuppliersModule.Suppliers.Models;
using FluentValidation;

namespace SuppliersModule.Suppliers.Features.Suppliers.UpdateSupplier;

public record UpdateSupplierCommand(SupplierDto Supplier) : ICommand<UpdateSupplierResult>;
public record UpdateSupplierResult(bool IsSuccess);

public class UpdateSupplierCommandValidator : AbstractValidator<UpdateSupplierCommand>
{
    public UpdateSupplierCommandValidator()
    {
        RuleFor(x => x.Supplier.Id).NotEmpty();
        RuleFor(x => x.Supplier.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Supplier.SupplierCode).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Supplier.CommercialName).MaximumLength(200);
        RuleFor(x => x.Supplier.TaxNumber).MaximumLength(50);
        RuleFor(x => x.Supplier.CreditLimit).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Supplier.OpeningBalance).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Supplier.Notes).MaximumLength(2000);
        RuleFor(x => x.Supplier.Status).IsInEnum();
        RuleFor(x => x.Supplier.Type).IsInEnum();
        RuleFor(x => x.Supplier.PaymentTerm).IsInEnum();
        RuleFor(x => x.Supplier.Addresses).NotNull();
        RuleFor(x => x.Supplier.Contacts).NotNull();
        RuleFor(x => x.Supplier.Addresses.Count(a => a.IsDefaultBilling)).LessThanOrEqualTo(1).WithMessage("Only one default billing address is allowed");
        RuleFor(x => x.Supplier.Contacts.Count(c => c.IsPrimaryContact)).LessThanOrEqualTo(1).WithMessage("Only one primary contact is allowed");

        RuleForEach(x => x.Supplier.Addresses).ChildRules(address =>
        {
            address.RuleFor(x => x.Title).NotEmpty().MaximumLength(100);
            address.RuleFor(x => x.AddressLine1).NotEmpty().MaximumLength(300);
            address.RuleFor(x => x.AddressLine2).MaximumLength(300);
            address.RuleFor(x => x.City).NotEmpty().MaximumLength(100);
            address.RuleFor(x => x.State).NotEmpty().MaximumLength(100);
            address.RuleFor(x => x.Country).NotEmpty().MaximumLength(100);
            address.RuleFor(x => x.PostalCode).MaximumLength(30);
        });

        RuleForEach(x => x.Supplier.Contacts).ChildRules(contact =>
        {
            contact.RuleFor(x => x.FullName).NotEmpty().MaximumLength(150);
            contact.RuleFor(x => x.JobTitle).MaximumLength(100);
            contact.RuleFor(x => x.Email).MaximumLength(200).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
            contact.RuleFor(x => x.PhoneNumber).MaximumLength(50);
        });
    }
}

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
            request.Supplier.PayableAccountId,
            request.Supplier.ExpenseAccountId,
            request.Supplier.DefaultCurrencyId,
            request.Supplier.FiscalPosition,
            request.Supplier.VendorPaymentReference,
            request.Supplier.Notes,
            request.Supplier.Addresses,
            request.Supplier.Contacts,
            user);

        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateSupplierResult(true);
    }
}
