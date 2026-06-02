using CustomersModule.Customers.Models;

namespace CustomersModule.Customers.Features.Customers.UpdateCustomer;

public record UpdateCustomerCommand(CustomerDto Customer) : ICommand<UpdateCustomerResult>;
public record UpdateCustomerResult(bool IsSuccess);
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
            request.Customer.CommercialName,
            request.Customer.Status,
            request.Customer.CustomerGroupId,
            //request.Customer.Type,
            request.Customer.CreditLimit,
            //request.Customer.PaymentTerm,
            request.Customer.Notes,
            request.Customer.IsTaxExempt,
            request.Customer.Addresses,
            request.Customer.Contacts,
            user);

        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateCustomerResult(true);
    }
}
