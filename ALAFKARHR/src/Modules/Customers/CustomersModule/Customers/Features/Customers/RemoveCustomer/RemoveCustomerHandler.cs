namespace CustomersModule.Customers.Features.Customers.RemoveCustomer;

public record RemoveCustomerCommand(Guid Id) : ICommand<RemoveCustomerResult>;
public record RemoveCustomerResult(bool IsSuccess);
public class RemoveCustomerHandler(CustomerDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<RemoveCustomerCommand, RemoveCustomerResult>
{
    public async Task<RemoveCustomerResult> Handle(RemoveCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer= await dbContext.Customers.FirstOrDefaultAsync(c=>c.Id==request.Id,cancellationToken);
        if (customer is null)
            throw new NotFoundException($"Customer not found: {request.Id}");

        var user = httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.NameIdentifier)?
                    .Value ??
                    throw new UnauthorizedAccessException("User is not authenticated");

        customer.Remove(user);
        return new RemoveCustomerResult(true);
    }
}
