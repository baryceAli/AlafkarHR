using CustomersModule.Customers.Models;

namespace CustomersModule.Customers.Features.Customers.CreateCustomer;

public record CreateCustomerCommand(CustomerDto Customer) : ICommand<CreateCustomerResult>;
public record CreateCustomerResult(Guid Id);
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
                command.Customer.CommercialName,
                command.Customer.Status,
                //command.Customer.Type,
                command.Customer.CreditLimit,
                //command.Customer.PaymentTerm,
                command.Customer.Notes,
                command.Customer.IsTaxExempt,
                command.Customer.CompanyId.Value,
                command.Customer.CustomerGroupId,
                user);

        if(command.Customer.Addresses.Any())
        {
            foreach(var add in command.Customer.Addresses)
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
                        add.IsDefaultShipping,
                        user);
                
            }
        }

        if (command.Customer.Contacts.Any())
        {
            foreach(var contact in command.Customer.Contacts)
            {
                customer.AddContact(
                    contact.FullName,
                    contact.JobTitle,
                    contact.Email,
                    contact.PhoneNumber,
                    contact.IsPrimaryContact,
                    user);
            }
        }

        await dbContext.Customers.AddAsync(customer,cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateCustomerResult(customer.Id);
    }
}
