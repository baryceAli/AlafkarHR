using CustomersModule.Customers.Models;
using FluentValidation;

namespace CustomersModule.Customers.Features.Customers.UpdateCustomer;

public record UpdateCustomerCommand(CustomerDto Customer) : ICommand<UpdateCustomerResult>;
public record UpdateCustomerResult(bool IsSuccess);

public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerCommandValidator()
    {
        RuleFor(x => x.Customer.Id).NotEmpty();
        RuleFor(x => x.Customer.Name).NotEmpty().MaximumLength(200).WithMessage("Customer name is required");
        RuleFor(x => x.Customer.CustomerCode).MaximumLength(50);
        RuleFor(x => x.Customer.CommercialName).MaximumLength(200);
        RuleFor(x => x.Customer.VatNumber).MaximumLength(50);
        RuleFor(x => x.Customer.CommercialRegistrationNumber).MaximumLength(50);
        RuleFor(x => x.Customer.CreditLimit).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Customer.AvailableCredit).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Customer.CreditHoldReason).MaximumLength(500);
        RuleFor(x => x.Customer.Notes).MaximumLength(2000);
        RuleFor(x => x.Customer.Status).IsInEnum();
        RuleFor(x => x.Customer.PaymentTerm).IsInEnum();
        RuleFor(x => x.Customer.CreditStatus).IsInEnum();
        RuleFor(x => x.Customer.Addresses).NotNull();
        RuleFor(x => x.Customer.Contacts).NotNull();
        RuleFor(x => x.Customer.Addresses.Count(a => a.IsDefaultBilling)).LessThanOrEqualTo(1).WithMessage("Only one default billing address is allowed");
        RuleFor(x => x.Customer.Addresses.Count(a => a.IsDefaultShipping)).LessThanOrEqualTo(1).WithMessage("Only one default shipping address is allowed");
        RuleFor(x => x.Customer.Contacts.Count(c => c.IsPrimaryContact)).LessThanOrEqualTo(1).WithMessage("Only one primary contact is allowed");

        RuleForEach(x => x.Customer.Addresses).ChildRules(address =>
        {
            address.RuleFor(x => x.Title).NotEmpty().MaximumLength(100);
            address.RuleFor(x => x.AddressLine1).NotEmpty().MaximumLength(300);
            address.RuleFor(x => x.AddressLine2).MaximumLength(300);
            address.RuleFor(x => x.City).NotEmpty().MaximumLength(100);
            address.RuleFor(x => x.State).NotEmpty().MaximumLength(100);
            address.RuleFor(x => x.Country).NotEmpty().MaximumLength(100);
            address.RuleFor(x => x.PostalCode).MaximumLength(30);
        });

        RuleForEach(x => x.Customer.Contacts).ChildRules(contact =>
        {
            contact.RuleFor(x => x.FullName).NotEmpty().MaximumLength(150);
            contact.RuleFor(x => x.JobTitle).MaximumLength(100);
            contact.RuleFor(x => x.Email).MaximumLength(200).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
            contact.RuleFor(x => x.PhoneNumber).MaximumLength(50);
        });
    }
}

public class UpdateCustomerHandler(CustomerDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateCustomerCommand, UpdateCustomerResult>
{
    public async Task<UpdateCustomerResult> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await dbContext.Customers.FirstOrDefaultAsync(c => c.Id == request.Customer.Id, cancellationToken);
        if (customer is null)
            throw new NotFoundException($"Customer is not found: {request.Customer.Id}");

        var user = httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.NameIdentifier)?
                    .Value ??
                    throw new UnauthorizedAccessException("User is not authenticated");

        customer.Update(
            request.Customer.Name,
            request.Customer.CustomerCode,
            request.Customer.CommercialName,
            request.Customer.VatNumber,
            request.Customer.CommercialRegistrationNumber,
            request.Customer.Status,
            request.Customer.CustomerGroupId,
            request.Customer.CreditLimit,
            request.Customer.PaymentTerm,
            request.Customer.CreditStatus,
            request.Customer.CreditHoldReason,
            request.Customer.AvailableCredit,
            request.Customer.Notes,
            request.Customer.IsTaxExempt,
            request.Customer.Addresses,
            request.Customer.Contacts,
            user);

        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateCustomerResult(true);
    }
}
