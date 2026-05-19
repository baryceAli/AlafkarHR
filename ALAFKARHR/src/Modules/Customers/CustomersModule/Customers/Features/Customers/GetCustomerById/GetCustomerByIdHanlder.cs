namespace CustomersModule.Customers.Features.Customers.GetCustomerById;

public record GetCustomerByIdQuery(Guid Id) : IQuery<GetCustomerByIdResult>;
public record GetCustomerByIdResult(CustomerDto Customer);
public class GetCustomerByIdHanlder(CustomerDbContext dbContext)
    : IQueryHandler<GetCustomerByIdQuery, GetCustomerByIdResult>
{
    public async Task<GetCustomerByIdResult> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await dbContext.Customers.Include(c=>c.Addresses).Include(c=>c.Contacts).AsNoTracking().FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        if (customer is null)
            throw new NotFoundException($"Customer not found: {request.Id}");

        return new GetCustomerByIdResult(customer.Adapt<CustomerDto>());
    }
}
