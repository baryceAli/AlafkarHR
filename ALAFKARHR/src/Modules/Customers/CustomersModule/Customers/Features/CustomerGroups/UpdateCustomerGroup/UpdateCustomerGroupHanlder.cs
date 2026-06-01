namespace CustomersModule.Customers.Features.CustomerGroups.UpdateCustomerGroup;

public record UpdateCustomerGroupCommand(CustomerGroupDto CustomerGroup) : ICommand<UpdateCustomerGroupResult>;
public record UpdateCustomerGroupResult(bool IsSuccess);
public class UpdateCustomerGroupHanlder(CustomerDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateCustomerGroupCommand, UpdateCustomerGroupResult>
{
    public async Task<UpdateCustomerGroupResult> Handle(UpdateCustomerGroupCommand request, CancellationToken cancellationToken)
    {
        var customerGroup = await dbContext.CustomerGroups.FirstOrDefaultAsync(c => c.Id == request.CustomerGroup.Id, cancellationToken);

        if (customerGroup is null)
            throw new NotFoundException($"Customer group not found: {request.CustomerGroup.Id}");

        var user = httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.NameIdentifier)?
                    .Value ??
                    throw new UnauthorizedAccessException("User is not authenticated");

        customerGroup.Update(request.CustomerGroup.Name,
                    request.CustomerGroup.NameEng,
                    request.CustomerGroup.Description,
                    request.CustomerGroup.DefaultDiscountPercentage,
                    request.CustomerGroup.DefaultPriceListId,
                    user);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new UpdateCustomerGroupResult(true);
    }
}
