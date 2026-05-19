
namespace CustomersModule.Customers.Features.CustomerGroups.GetCustomerGroupById;

public record GetCustomerGroupByIdQuery(Guid Id) : IQuery<GetCustomerGroupByIdResult>;
public record GetCustomerGroupByIdResult(CustomerGroupDto CustomerGroup);
public class GetCustomerGroupByIdHandler(CustomerDbContext dbContext)
    : IQueryHandler<GetCustomerGroupByIdQuery, GetCustomerGroupByIdResult>
{
    public async Task<GetCustomerGroupByIdResult> Handle(GetCustomerGroupByIdQuery request, CancellationToken cancellationToken)
    {
        var customerGroup = await dbContext.CustomerGroups.AsNoTracking().FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        if (customerGroup is null)
            throw new NotFoundException($"Customer group not found: {request.Id}");
    
        return new GetCustomerGroupByIdResult(customerGroup.Adapt<CustomerGroupDto>());
    }
}
