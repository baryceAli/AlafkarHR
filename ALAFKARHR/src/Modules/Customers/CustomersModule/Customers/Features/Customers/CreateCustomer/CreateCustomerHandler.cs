using CustomersModule.Customers.Models;
using FluentValidation;

namespace CustomersModule.Customers.Features.Customers.CreateCustomer;

public record CreateCustomerCommand(CustomerDto Customer) : ICommand<CreateCustomerResult>;
public record CreateCustomerResult(Guid Id);

public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(x => x.Customer.CompanyId).NotNull().WithMessage("Company is required");
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

public class CreateCustomerHandler(CustomerDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CreateCustomerCommand, CreateCustomerResult>
{
    public async Task<CreateCustomerResult> Handle(CreateCustomerCommand command, CancellationToken cancellationToken)
    {
        var user = httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.NameIdentifier)?
                    .Value ??
                    throw new UnauthorizedAccessException("User is not authenticated");

        var customer = Customer.Create(
            Guid.NewGuid(),
            command.Customer.Name,
            command.Customer.CustomerCode,
            command.Customer.CommercialName,
            command.Customer.VatNumber,
            command.Customer.CommercialRegistrationNumber,
            command.Customer.Status,
            command.Customer.CreditLimit,
            command.Customer.PaymentTerm,
            command.Customer.CreditStatus,
            command.Customer.CreditHoldReason,
            command.Customer.AvailableCredit,
            command.Customer.Notes,
            command.Customer.IsTaxExempt,
            command.Customer.ReceivableAccountId,
            command.Customer.IncomeAccountId,
            command.Customer.DefaultCurrencyId,
            command.Customer.FiscalPosition,
            command.Customer.CustomerPaymentReference,
            command.Customer.CompanyId.Value,
            command.Customer.CustomerGroupId,
            user);

        foreach (var add in command.Customer.Addresses)
        {
            customer.AddAddress(
                add.Title,
                add.AddressLine1,
                add.AddressLine2,
                add.Longitude,
                add.Latitude,
                add.City,
                add.State,
                add.Country,
                add.PostalCode,
                add.IsDefaultBilling,
                add.IsDefaultShipping,
                add.AddressType,
                user);
        }

        foreach (var contact in command.Customer.Contacts)
        {
            customer.AddContact(
                contact.FullName,
                contact.JobTitle,
                contact.Email,
                contact.PhoneNumber,
                contact.IsPrimaryContact,
                contact.ContactType,
                user);
        }

        await dbContext.Customers.AddAsync(customer, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateCustomerResult(customer.Id);
    }
}
