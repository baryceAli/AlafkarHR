namespace CustomersModule.Customers.Features.CustomerPricingProfiles.RemoveCustomerPricingProfiles;


public record RemoveCustomerPricingProfilesCommand(Guid Id) : ICommand<RemoveCustomerPricingProfilesResult>;
public record RemoveCustomerPricingProfilesResult(bool IsSuccess);
public class RemoveCustomerPricingProfilesHandler(CustomerDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<RemoveCustomerPricingProfilesCommand, RemoveCustomerPricingProfilesResult>
{
    public async Task<RemoveCustomerPricingProfilesResult> Handle(RemoveCustomerPricingProfilesCommand command, CancellationToken cancellationToken)
    {
        var customerPricingProfile=await dbContext.CustomerPricingProfiles.FirstOrDefaultAsync(c=>c.Id==command.Id,cancellationToken);

        if (customerPricingProfile is null)
            throw new NotFoundException($"Customer pricing profile not found: {command.Id}");

        var user = httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.NameIdentifier)?
                    .Value ??
                    throw new UnauthorizedAccessException("User is not authenticated");

        customerPricingProfile.Remove(user);

        await dbContext.SaveChangesAsync();

        return new RemoveCustomerPricingProfilesResult(true);
    }
}
