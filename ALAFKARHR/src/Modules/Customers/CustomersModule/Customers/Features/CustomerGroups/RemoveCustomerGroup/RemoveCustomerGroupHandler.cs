namespace CustomersModule.Customers.Features.CustomerGroups.RemoveCustomerGroup;

public record RemoveCustomerGroupCommand(Guid Id) : ICommand<RemoveCustomerGroupResult>;
public record RemoveCustomerGroupResult(bool IsSuccess);
public class RemoveCustomerGroupHandler(CustomerDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<RemoveCustomerGroupCommand, RemoveCustomerGroupResult>
{
    public async Task<RemoveCustomerGroupResult> Handle(RemoveCustomerGroupCommand request, CancellationToken cancellationToken)
    {
        var customer=await dbContext.CustomerGroups.AsNoTracking().FirstOrDefaultAsync(c=>c.Id==request.Id,cancellationToken);

        if (customer == null)
            throw new NotFoundException($"Customer group not found: {request.Id}");

        var user = httpContextAccessor.HttpContext
                    .User?
                    .FindFirst(ClaimTypes.NameIdentifier)
                    .Value ??
                    throw new UnauthorizedAccessException("User is not authenticated");

        customer.Remove(user);

        await dbContext.SaveChangesAsync();

        return new RemoveCustomerGroupResult(true);
    }
}
